import { useState, useEffect } from 'react';
import { getAllRequests, assignManager, takeRequest, completeRequest } from '../../api/requestsApi';
import { getAllUsers } from '../../api/usersApi';

const STATUS_LABELS = {
    accepted: 'Принята',
    new: 'Принята',
    assigned: 'Назначена',
    in_progress: 'В работе',
    done: 'Выполнена'
};

function getCommentText(task) {
    // У системных запросов новое значение показываем в комментарии
    return task.newValue || task.comment || '—';
}

function getResponsibleLabel(task, managers = []) {
    if (task.managerFio) return task.managerFio;
    const m = managers.find(x => x.id === task.managerId);
    if (m) return m.fio || m.tabn || '—';
    return '—';
}

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

    const currentUserId = localStorage.getItem('userId')
        ? parseInt(localStorage.getItem('userId'), 10)
        : null;

    useEffect(() => {
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

    async function refreshTasks(keepSelectedId) {
        const data = await getAllRequests();
        setTasks(data);
        if (keepSelectedId != null) {
            const refreshed = data.find(t => t.id === keepSelectedId);
            if (refreshed) {
                setSelected(refreshed);
                setSelectedManagerId(refreshed.managerId?.toString() || '');
                setResolutionComment(refreshed.resolutionComment || '');
            }
        }
        return data;
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

    async function handleAssign() {
        if (!selectedManagerId) return;
        setActionLoading(true);
        setActionError(null);
        try {
            await assignManager(selected.id, parseInt(selectedManagerId, 10));
            await refreshTasks(selected.id);
        } catch (err) {
            setActionError(err.message);
        } finally {
            setActionLoading(false);
        }
    }

    async function handleTake() {
        setActionLoading(true);
        setActionError(null);
        try {
            await takeRequest(selected.id);
            await refreshTasks(selected.id);
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
            await refreshTasks(selected.id);
        } catch (err) {
            setActionError(err.message);
        } finally {
            setActionLoading(false);
        }
    }

    const formatDate = (d) => (d ? new Date(d).toLocaleDateString('ru-RU') : '—');

    if (loading) return <div>Загрузка...</div>;
    if (error) return <div>Ошибка: {error}</div>;

    if (selected) {
        const isAssignedToMe = currentUserId && selected.managerId === currentUserId;
        const isDone = selected.status === 'done';

        return (
            <div>
                <button type="button" onClick={handleClose}>← Назад</button>

                <h2>Задача #{selected.id}</h2>

                <p><strong>Тип:</strong> {selected.requestTypeName}</p>
                <p><strong>Сотрудник:</strong> {selected.employeeFio || selected.employeeTabn || '—'}</p>
                <p><strong>Комментарий:</strong> {getCommentText(selected)}</p>
                <p><strong>Статус:</strong> {STATUS_LABELS[selected.status] || selected.status}</p>

                {!isDone ? (
                    <p>
                        <strong>Ответственный:</strong>{' '}
                        <select
                            value={selectedManagerId}
                            onChange={(e) => setSelectedManagerId(e.target.value)}
                            disabled={actionLoading}
                            style={{
                                fontSize: 14,
                                padding: '2px 6px',
                                maxWidth: 260,
                                verticalAlign: 'middle'
                            }}
                        >
                            <option value="">— выбрать —</option>
                            {managers.map(m => (
                                <option key={m.id} value={m.id}>
                                    {m.fio || m.tabn}{m.id === currentUserId ? ' (Вы)' : ''}
                                </option>
                            ))}
                        </select>
                        <button
                            type="button"
                            onClick={handleAssign}
                            disabled={actionLoading || !selectedManagerId}
                            style={{ marginLeft: 8, fontSize: 14, padding: '2px 10px' }}
                        >
                            Назначить
                        </button>
                    </p>
                ) : (
                    <p><strong>Ответственный:</strong> {getResponsibleLabel(selected, managers)}</p>
                )}

                <p><strong>Дата создания:</strong> {formatDate(selected.createdAt)}</p>

                {isDone && (
                    <p><strong>Описание решения:</strong> {selected.resolutionComment || '—'}</p>
                )}

                {isAssignedToMe && !isDone && (selected.status === 'assigned' || selected.status === 'new') && (
                    <div style={{ marginTop: 16 }}>
                        <button type="button" onClick={handleTake} disabled={actionLoading}>
                            В работу
                        </button>
                    </div>
                )}

                {isAssignedToMe && !isDone && selected.status === 'in_progress' && (
                    <div style={{ marginTop: 16 }}>
                        <div className="form-group">
                            <label htmlFor="resolution">Описание решения:</label>
                            <textarea
                                id="resolution"
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
                        >
                            Выполнить
                        </button>
                    </div>
                )}

                {actionError && <p style={{ color: 'red', marginTop: 10 }}>{actionError}</p>}
            </div>
        );
    }

    return (
        <div>
            <h2>Задачи</h2>

            {tasks.length === 0 ? (
                <p>Нет задач</p>
            ) : (
                <div style={{ display: 'flex', justifyContent: 'center' }}>
                    <table className="table">
                        <thead>
                            <tr>
                                <th>№</th>
                                <th>Тип</th>
                                <th>Сотрудник</th>
                                <th>Комментарии</th>
                                <th>Статус</th>
                                <th>Ответственный</th>
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
                                    <td>{getCommentText(task)}</td>
                                    <td>{STATUS_LABELS[task.status] || task.status}</td>
                                    <td>{getResponsibleLabel(task, managers)}</td>
                                    <td>{formatDate(task.createdAt)}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}
