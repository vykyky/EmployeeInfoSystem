import { useState, useEffect } from 'react';
import { getAllRequests, takeRequest, completeRequest } from '../../api/requestsApi';

const STATUS_LABELS = {
    new: 'Новая',
    in_progress: 'В работе',
    done: 'Выполнена'
};

export default function Tasks() {
    const [tasks, setTasks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Выбранная задача для редактирования
    const [selected, setSelected] = useState(null);
    const [resolutionComment, setResolutionComment] = useState('');
    const [actionLoading, setActionLoading] = useState(false);
    const [actionError, setActionError] = useState(null);

    useEffect(() => {
        loadTasks();
    }, []);

    function loadTasks() {
        setLoading(true);
        getAllRequests()
            .then(data => {
                setTasks(data);
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    }

    function handleSelect(task) {
        setSelected(task);
        setResolutionComment('');
        setActionError(null);
    }

    function handleClose() {
        setSelected(null);
        setActionError(null);
    }

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

    const formatDate = (dateString) => {
        if (!dateString) return '—';
        return new Date(dateString).toLocaleDateString('ru-RU');
    };

    if (loading) return <div>Загрузка задач...</div>;
    if (error) return <div>Ошибка: {error}</div>;

    // Режим редактирования задачи (Рис. 16)
    if (selected) {
        return (
            <div>
                <button type="button" onClick={handleClose}>← Назад</button>

                <h2>Задача #{selected.id}</h2>

                <p><strong>Тип:</strong> {selected.requestTypeName}</p>
                <p><strong>Сотрудник:</strong> {selected.employeeFio || selected.employeeTabn || '—'}</p>
                <p><strong>Комментарий:</strong> {selected.comment || '—'}</p>
                <p><strong>Новое значение:</strong> {selected.newValue || '—'}</p>
                <p><strong>Статус:</strong> {STATUS_LABELS[selected.status] || selected.status}</p>
                <p><strong>Дата создания:</strong> {formatDate(selected.createdAt)}</p>

                <div>
                    <label>Описание решения</label>
                    <textarea
                        value={resolutionComment}
                        onChange={(e) => setResolutionComment(e.target.value)}
                        placeholder="Опишите выполненное действие..."
                        rows={4}
                        disabled={selected.status === 'done' || actionLoading}
                    />
                </div>

                {selected.status !== 'done' && (
                    <div>
                        {selected.status === 'new' && (
                            <button
                                type="button"
                                onClick={handleTake}
                                disabled={actionLoading}
                            >
                                В работу
                            </button>
                        )}
                        {selected.status === 'in_progress' && (
                            <button
                                type="button"
                                onClick={handleComplete}
                                disabled={actionLoading || !resolutionComment.trim()}
                            >
                                Завершить
                            </button>
                        )}
                    </div>
                )}

                {selected.status === 'done' && (
                    <p><strong>Решение:</strong> {selected.resolutionComment || '—'}</p>
                )}

                {actionError && <p style={{ color: 'red' }}>{actionError}</p>}
            </div>
        );
    }

    // Список задач (Рис. 15)
    return (
        <div>
            <h2>Задачи</h2>

            {tasks.length === 0 ? (
                <p>Нет назначенных задач</p>
            ) : (
                <table border="1">
                    <thead>
                        <tr>
                            <th>№</th>
                            <th>Тип</th>
                            <th>Сотрудник</th>
                            <th>Новое значение</th>
                            <th>Статус</th>
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
                                <td>{formatDate(task.createdAt)}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}
        </div>
    );
}