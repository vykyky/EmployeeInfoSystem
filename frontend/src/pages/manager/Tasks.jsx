import { useState, useEffect } from 'react';
import { getAllRequests, takeRequest, completeRequest } from '../../api/requestsApi';

const STATUS_LABELS = {
    accepted: 'Принята',
    new: 'Принята',
    assigned: 'Назначена',
    in_progress: 'В работе',
    done: 'Выполнена'
};

function getCommentText(task) {
    return task.newValue || task.comment || '—';
}

export default function Tasks() {
    const [tasks, setTasks] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

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

    async function refreshAndKeepSelected(keepSelectedId) {
        const data = await getAllRequests();
        setTasks(data);
        if (keepSelectedId != null) {
            const refreshed = data.find(t => t.id === keepSelectedId);
            if (refreshed) {
                setSelected(refreshed);
                setResolutionComment(refreshed.resolutionComment || '');
            }
        }
    }

    function handleSelect(task) {
        setSelected(task);
        setResolutionComment(task.resolutionComment || '');
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
            await refreshAndKeepSelected(selected.id);
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
            await refreshAndKeepSelected(selected.id);
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

    if (selected) {
        const isDone = selected.status === 'done';

        return (
            <div>
                <button type="button" onClick={handleClose}>← Назад</button>

                <h2>Задача #{selected.id}</h2>

                <p><strong>Тип:</strong> {selected.requestTypeName}</p>
                <p><strong>Сотрудник:</strong> {selected.employeeFio || selected.employeeTabn || '—'}</p>
                <p><strong>Комментарий:</strong> {getCommentText(selected)}</p>
                <p><strong>Статус:</strong> {STATUS_LABELS[selected.status] || selected.status}</p>
                <p><strong>Ответственный:</strong> {selected.managerFio || '—'}</p>
                <p><strong>Дата создания:</strong> {formatDate(selected.createdAt)}</p>

                {isDone && (
                    <p><strong>Описание решения:</strong> {selected.resolutionComment || '—'}</p>
                )}

                {!isDone && (selected.status === 'assigned' || selected.status === 'new') && (
                    <div style={{ marginTop: 16 }}>
                        <button type="button" onClick={handleTake} disabled={actionLoading}>
                            В работу
                        </button>
                    </div>
                )}

                {!isDone && selected.status === 'in_progress' && (
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

                {actionError && <p style={{ color: 'red' }}>{actionError}</p>}
            </div>
        );
    }

    return (
        <div>
            <h2>Задачи</h2>

            {tasks.length === 0 ? (
                <p>Нет назначенных задач</p>
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
                                    <td>{task.managerFio || '—'}</td>
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
