import { useState, useEffect } from 'react';
import { getAllRequests, takeRequest, completeRequest } from '../../api/requestsApi';
import './Tasks.css';

const STATUS_LABELS = {
  accepted: 'Принята',
  new: 'Принята',
  assigned: 'Назначена',
  in_progress: 'В работе',
  done: 'Выполнена',
};

const STATUS_CLASS = {
  accepted: 'status-new',
  new: 'status-new',
  assigned: 'status-assigned',
  in_progress: 'status-in-progress',
  done: 'status-done',
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
      .then(data => { setTasks(data); setLoading(false); })
      .catch(err => { setError(err.message); setLoading(false); });
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
    if (!resolutionComment.trim()) { setActionError('Укажите описание решения'); return; }
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

  const formatDate = (d) => (d ? new Date(d).toLocaleDateString('ru-RU') : '—');

  if (loading) return <div className="page">Загрузка...</div>;
  if (error) return <div className="page tasks-error">Ошибка: {error}</div>;

  if (selected) {
    const isDone = selected.status === 'done';

    return (
      <div className="page">
       <div className="page-header">
            <button className="btn btn-secondary" onClick={handleClose}>
            Назад
          </button>
        </div>

        <div className="task-detail-card">
          <div className="task-detail-header">
            <span className="task-detail-id">Задача #{selected.id}</span>
            <span className={`task-status-badge ${STATUS_CLASS[selected.status] || ''}`}>
              {STATUS_LABELS[selected.status] || selected.status}
            </span>
          </div>

          <div className="task-detail-grid">
            <div className="tdg-label">Тип</div>
            <div className="tdg-value">{selected.requestTypeName}</div>

            <div className="tdg-label">Сотрудник</div>
            <div className="tdg-value">{selected.employeeFio || selected.employeeTabn || '—'}</div>

            <div className="tdg-label">Комментарий</div>
            <div className="tdg-value">{getCommentText(selected)}</div>

            <div className="tdg-label">Ответственный</div>
            <div className="tdg-value">{selected.managerFio || '—'}</div>

            <div className="tdg-label">Дата создания</div>
            <div className="tdg-value">{formatDate(selected.createdAt)}</div>

            {isDone && (
              <>
                <div className="tdg-label">Решение</div>
                <div className="tdg-value">{selected.resolutionComment || '—'}</div>
              </>
            )}
          </div>

          {!isDone && (selected.status === 'assigned' || selected.status === 'new') && (
            <div className="task-detail-actions">
              <button className="btn btn-primary" style={{ fontSize: 15 }} onClick={handleTake} disabled={actionLoading}>
                В работу
              </button>
            </div>
          )}

          {!isDone && selected.status === 'in_progress' && (
            <div className="task-detail-actions">
              <div className="form-group">
                <label>Описание решения</label>
                <textarea
                  value={resolutionComment}
                  onChange={(e) => setResolutionComment(e.target.value)}
                  placeholder="Опишите выполненное действие..."
                  rows={4}
                  disabled={actionLoading}
                />
              </div>
              <button
                className="btn btn-primary"
                style={{ fontSize: 15 }}
                onClick={handleComplete}
                disabled={actionLoading || !resolutionComment.trim()}
              >
                Выполнить
              </button>
            </div>
          )}

          {actionError && <p className="task-action-error">{actionError}</p>}
        </div>
      </div>
    );
  }

  return (
    <div className="page">
      {tasks.length === 0 ? (
        <p className="tasks-empty">Нет назначенных задач</p>
      ) : (
        <div className="tasks-table-wrap">
          <table className="table tasks-table">
            <thead>
              <tr>
                <th>№</th>
                <th>Тип</th>
                <th>Сотрудник</th>
                <th>Комментарий</th>
                <th>Статус</th>
                <th>Ответственный</th>
                <th>Дата</th>
              </tr>
            </thead>
            <tbody>
              {tasks.map(task => (
                <tr key={task.id} className="tasks-row" onClick={() => handleSelect(task)}>
                  <td>{task.id}</td>
                  <td>{task.requestTypeName}</td>
                  <td>{task.employeeFio || task.employeeTabn || '—'}</td>
                  <td>{getCommentText(task)}</td>
                  <td>
                    <span className={`task-status-badge ${STATUS_CLASS[task.status] || ''}`}>
                      {STATUS_LABELS[task.status] || task.status}
                    </span>
                  </td>
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