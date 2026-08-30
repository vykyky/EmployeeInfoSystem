import { useState, useEffect } from 'react';
import {
    getAllRequestTypes,
    createRequestType,
    updateRequestType,
    deleteRequestType
} from '../../api/requestTypesApi';
import './RequestTypesAdmin.css';

export default function RequestTypes() {
    const [types, setTypes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Форма добавления
    const [showForm, setShowForm] = useState(false);
    const [newName, setNewName] = useState('');
    const [newActive, setNewActive] = useState(true);
    const [createError, setCreateError] = useState(null);
    const [creating, setCreating] = useState(false);

    // Inline редактирование
    const [editingId, setEditingId] = useState(null);
    const [editName, setEditName] = useState('');
    const [editActive, setEditActive] = useState(true);
    const [editError, setEditError] = useState(null);
    const [editLoading, setEditLoading] = useState(false);

    useEffect(() => {
        loadTypes();
    }, []);

    // Закрытие модального окна по нажатию клавиши Escape
    useEffect(() => {
        function handleKeyDown(e) {
            if (e.key === 'Escape' && showForm) {
                handleCancelForm();
            }
        }
        window.addEventListener('keydown', handleKeyDown);
        return () => window.removeEventListener('keydown', handleKeyDown);
    }, [showForm]);

    function loadTypes() {
        setLoading(true);
        getAllRequestTypes()
            .then(data => {
                setTypes(data);
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    }

    async function handleCreate(e) {
        e.preventDefault();
        if (!newName.trim()) {
            setCreateError('Введите название');
            return;
        }
        setCreating(true);
        setCreateError(null);
        try {
            await createRequestType({ name: newName.trim(), isActive: newActive });
            setNewName('');
            setNewActive(true);
            setShowForm(false);
            loadTypes();
        } catch (err) {
            setCreateError(err.message);
        } finally {
            setCreating(false);
        }
    }

    function handleCancelForm() {
        setShowForm(false);
        setNewName('');
        setNewActive(true);
        setCreateError(null);
    }

    function startEdit(type) {
        setEditingId(type.id);
        setEditName(type.name);
        setEditActive(type.isActive);
        setEditError(null);
    }

    function cancelEdit() {
        setEditingId(null);
        setEditError(null);
    }

    async function handleUpdate(type) {
        if (!editName.trim()) {
            setEditError('Введите название');
            return;
        }
        setEditLoading(true);
        setEditError(null);
        try {
            await updateRequestType(type.id, { name: editName.trim(), isActive: editActive });
            setEditingId(null);
            loadTypes();
        } catch (err) {
            setEditError(err.message);
        } finally {
            setEditLoading(false);
        }
    }

    async function handleDelete(id) {
        if (!window.confirm('Удалить тип запроса?')) return;
        try {
            await deleteRequestType(id);
            loadTypes();
        } catch (err) {
            alert(err.message);
        }
    }

    if (loading) return <div className="page">Загрузка...</div>;
    if (error) return <div className="page rt-error">Ошибка: {error}</div>;

    return (
        <div className="page">
            <div className="page-header">
                <button
                    className="btn btn-primary"
                    onClick={() => setShowForm(true)}
                >
                    Добавить тип
                </button>
            </div>

            {showForm && (
                <div className="modal-overlay" onClick={handleCancelForm}>
                    <div className="modal-card" onClick={(e) => e.stopPropagation()}>
                        <div className="modal-header">
                            <h2 className="modal-title">Новый тип запроса</h2>
                            <button
                                type="button"
                                className="modal-close-btn"
                                onClick={handleCancelForm}
                            >
                                ✕
                            </button>
                        </div>
                        <form onSubmit={handleCreate} noValidate>
                            <div className="form-group">
                                <label>Название</label>
                                <input
                                    value={newName}
                                    onChange={(e) => setNewName(e.target.value)}
                                    placeholder="Название типа"
                                    disabled={creating}
                                    autoComplete="off"
                                    autoFocus
                                />
                            </div>

                            <div className="form-group">
                                <label className="rt-checkbox-label">
                                    <input
                                        type="checkbox"
                                        checked={newActive}
                                        onChange={(e) => setNewActive(e.target.checked)}
                                        disabled={creating}
                                    />
                                    Активен
                                </label>
                            </div>

                            {createError && <p className="rt-field-error">{createError}</p>}

                            <div className="rt-form-actions">
                               
                                <button
                                    type="submit"
                                    className="btn btn-primary"
                                    disabled={creating || !newName.trim()}
                                >
                                    {creating ? "Добавление..." : "Добавить"}
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            <div className="rt-wrap">
                <table className="table types-table">
                    <thead>
                        <tr>
                            <th>Название</th>
                            <th>Активен</th>
                            <th>Системный</th>
                            <th>Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        {types.map(type => (
                            <tr key={type.id}>
                                {editingId === type.id ? (
                                    <>
                                        <td>
                                            <input
                                                className="rt-input"
                                                value={editName}
                                                onChange={(e) => setEditName(e.target.value)}
                                                disabled={editLoading}
                                            />
                                        </td>
                                        <td>
                                            <input
                                                type="checkbox"
                                                checked={editActive}
                                                onChange={(e) => setEditActive(e.target.checked)}
                                                disabled={editLoading || type.isSystem}
                                            />
                                        </td>
                                        <td>{type.isSystem ? 'Да' : 'Нет'}</td>
                                        <td>
                                            <button
                                                type="button"
                                                className="btn btn-primary rt-btn-sm"
                                                onClick={() => handleUpdate(type)}
                                                disabled={editLoading}
                                            >
                                                Сохранить
                                            </button>
                                            <button
                                                type="button"
                                                className="btn btn-secondary rt-btn-sm"
                                                onClick={cancelEdit}
                                                disabled={editLoading}
                                            >
                                                Отмена
                                            </button>
                                            {editError && <span className="rt-error">{editError}</span>}
                                        </td>
                                    </>
                                ) : (
                                    <>
                                        <td>{type.name}</td>
                                        <td>{type.isActive ? 'Да' : 'Нет'}</td>
                                        <td>{type.isSystem ? 'Да' : 'Нет'}</td>
                                        <td>
                                            <button
                                                type="button"
                                                className="btn btn-secondary rt-btn-sm"
                                                onClick={() => startEdit(type)}
                                            >
                                                Изменить
                                            </button>
                                            {!type.isSystem && (
                                                <button
                                                    type="button"
                                                    className="btn btn-delete rt-btn-sm"
                                                    onClick={() => handleDelete(type.id)}
                                                >
                                                    Удалить
                                                </button>
                                            )}
                                        </td>
                                    </>
                                )}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}