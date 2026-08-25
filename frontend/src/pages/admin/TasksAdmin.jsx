import { useState, useEffect } from 'react';
import { getAllRequests, assignManager, takeRequest, completeRequest } from '../../api/requestsApi';
import { getAllUsers } from '../../api/usersApi';

const STATUS_LABELS = {
    new: 'Новая',
    in_progress: 'В работе',
    done: 'Выполнена'
};

export default function AdminTasks() {
    const [tasks, setTasks] = useState([]);
    const [managers, setManagers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const [selected, setSelected] = useState(null);
    const [resolutionComment, setResolutionComment] = useState('');
    const [selectedManagerId, setSelectedManagerId] = useState('');
    const [actionLoading, setActionLoading] = useState(false);
    const [actionError, setActionError] = useState(null);

    // Читаем ID текущего админа прямо из localStorage
    const currentUserId = localStorage.getItem("userId") 
        ? parseInt(localStorage.getItem("userId"), 10) 
        : null;

    useEffect(() => {
        // Загружаем одновременно задачи и список всех пользователей для назначения
        Promise.all([getAllRequests(), getAllUsers()])
            .then(([taskData, userData]) => {
                setTasks(taskData);
                setManagers(userData.filter(u => u.role === 'manager' || u.role === 'admin'));
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    }, []);

    function loadTasks() {
        getAllRequests().then(setTasks).catch(() => {});
    }

    function handleSelect(task) {
        setSelected(task);
        setResolutionComment(task.resolutionComment || '');
        setSelectedManagerId(task.managerId?.toString() || '');
        setActionError(null);
    }

    function handleClose() {
        setSelected(null);
        setActionError(null);
    }

    // Назначение / Переназначение менеджера
    async function handleAssign() {
        if (!selectedManagerId) return;
        setActionLoading(true);
        setActionError(null);
        try {
            await assignManager(selected.id, parseInt(selectedManagerId, 10));
            loadTasks();
            handleClose();
        } catch (err) {
            setActionError(err.message);
        } finally {
            setActionLoading(false);
        }
    }

    // Взятие задачи в работу
    async function handleTake() {
        setActionLoading(true);
        setActionError(null);
        try {
            await takeRequest(selected.id);
            loadTasks();
            handleClose();
        } catch (err) {
            setActionError(err.message);
        } finally {
            setActionLoading(false);
        }
    }

    // Завершение задачи с фиксацией решения
    async function handleComplete() {
        if (!resolutionComment.trim()) {
            setActionError('Укажите описание решения');
            return;
        }
        setActionLoading(true);
        setActionError(null);
        try {
            await completeRequest(selected.id, resolutionComment);
            loadTasks();
            handleClose();
        } catch (err) {
            setActionError(err.message);
        } finally {
            setActionLoading(false);
        }
    }

    const formatDate = (d) => d ? new Date(d).toLocaleDateString('ru-RU') : '—';

    if (loading) return <div>Загрузка...</div>;
    if (error) return <div>Ошибка: {error}</div>;

    if (selected) {
        // Сверяем назначена ли задача на вошедшего админа
        const isAssignedToMe = currentUserId && selected.managerId === currentUserId;

        return (
            <div>
                <button type="button" onClick={handleClose}>← Назад</button>

                <h2>Задача #{selected.id}</h2>

                <p><strong>Тип:</strong> {selected.requestTypeName}</p>
                <p><strong>Сотрудник:</strong> {selected.employeeFio || selected.employeeTabn || '—'}</p>
                <p><strong>Комментарий:</strong> {selected.comment || '—'}</p>
                <p><strong>Новое значение:</strong> {selected.newValue || '—'}</p>
                <p><strong>Статус:</strong> {STATUS_LABELS[selected.status] || selected.status}</p>
                <p><strong>Ответственный менеджер:</strong> {selected.managerFio || '—'}</p>
                <p><strong>Дата создания:</strong> {formatDate(selected.createdAt)}</p>

                {/* 1. АДМИНИСТРАТИВНЫЙ БЛОК: Назначить / переназначить менеджера */}
                {selected.status !== 'done' && (
                    <div style={{ margin: '15px 0', padding: '10px', border: '1px solid #ccc' }}>
                        <h3>Управление ответственным</h3>
                        <label>Назначить менеджера: </label>
                        <select
                            value={selectedManagerId}
                            onChange={(e) => setSelectedManagerId(e.target.value)}
                            disabled={actionLoading}
                        >
                            <option value="">— выбрать —</option>
                            {managers.map(m => (
                                <option key={m.id} value={m.id}>
                                    {m.fio || m.tabn} {m.id === currentUserId ? '(Вы)' : ''}
                                </option>
                            ))}
                        </select>
                        <button
                            type="button"
                            onClick={handleAssign}
                            disabled={actionLoading || !selectedManagerId}
                            style={{ marginLeft: '10px' }}
                        >
                            Назначить
                        </button>
                    </div>
                )}

                {/* 2. ИСПОЛНИТЕЛЬСКИЙ БЛОК: Виден только если задача назначена на текущего админа */}
                {isAssignedToMe && selected.status !== 'done' && (
                    <div style={{ margin: '15px 0', padding: '10px', border: '1px solid #4CAF50' }}>
                        <h3>Работа над задачей</h3>
                        
                        {/* Статус: НОВЫЙ — показываем только кнопку "В работу" */}
                        {selected.status === 'new' && (
                            <button
                                type="button"
                                onClick={handleTake}
                                disabled={actionLoading}
                            >
                                В работу
                            </button>
                        )}

                        {/* Статус: В РАБОТЕ — показываем textarea и кнопку "Завершить" */}
                        {selected.status === 'in_progress' && (
                            <div>
                                <div>
                                    <label>Описание решения</label>
                                    <br />
                                    <textarea
                                        value={resolutionComment}
                                        onChange={(e) => setResolutionComment(e.target.value)}
                                        placeholder="Опишите выполненное действие..."
                                        rows={4}
                                        disabled={actionLoading}
                                    />
                                </div>
                                <button
                                    type="button"
                                    onClick={handleComplete}
                                    disabled={actionLoading || !resolutionComment.trim()}
                                    style={{ marginTop: '10px' }}
                                >
                                    Завершить
                                </button>
                            </div>
                        )}
                    </div>
                )}

                {/* 3. Если задача ЗАВЕРШЕНА — вывод итогового решения */}
                {selected.status === 'done' && (
                    <div style={{ marginTop: '15px' }}>
                        <p><strong>Описание решения:</strong> {selected.resolutionComment || '—'}</p>
                    </div>
                )}

                {actionError && <p style={{ color: 'red', marginTop: '10px' }}>{actionError}</p>}
            </div>
        );
    }

    return (
        <div>
            <h2>Задачи</h2>

            {tasks.length === 0 ? (
                <p>Нет задач</p>
            ) : (
                <table border="1">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Тип</th>
                            <th>Сотрудник</th>
                            <th>Новое значение</th>
                            <th>Статус</th>
                            <th>Менеджер</th>
                            <th>Дата</th>
                        </tr>
                    </thead>
                    <tbody>
                        {tasks.map(task => (
                            <tr
                                key={task.id}
                                onClick={() => handleSelect(task)}
                                style={{ cursor: 'pointer' }}
                            >
                                <td>{task.id}</td>
                                <td>{task.requestTypeName}</td>
                                <td>{task.employeeFio || task.employeeTabn || '—'}</td>
                                <td>{task.newValue || '—'}</td>
                                <td>{STATUS_LABELS[task.status] || task.status}</td>
                                <td>{task.managerFio || '—'}</td>
                                <td>{formatDate(task.createdAt)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}