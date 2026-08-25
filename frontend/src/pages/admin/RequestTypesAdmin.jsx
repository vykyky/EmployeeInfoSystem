import { useState, useEffect } from 'react';
import {
    getAllRequestTypes,
    createRequestType,
    updateRequestType,
    deleteRequestType
} from '../../api/requestTypesApi';

export default function RequestTypes() {
    const [types, setTypes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Форма добавления
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

    async function handleCreate() {
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
            loadTypes();
        } catch (err) {
            setCreateError(err.message);
        } finally {
            setCreating(false);
        }
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

    if (loading) return <div>Загрузка...</div>;
    if (error) return <div>Ошибка: {error}</div>;

    return (
        <div>
            <h2>Электронный запрос — типы</h2>

            <table border="1">
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
                                            onClick={() => handleUpdate(type)}
                                            disabled={editLoading}
                                        >
                                            Сохранить
                                        </button>
                                        <button
                                            type="button"
                                            onClick={cancelEdit}
                                            disabled={editLoading}
                                        >
                                            Отмена
                                        </button>
                                        {editError && <span style={{ color: 'red' }}> {editError}</span>}
                                    </td>
                                </>
                            ) : (
                                <>
                                    <td>{type.name}</td>
                                    <td>{type.isActive ? 'Да' : 'Нет'}</td>
                                    <td>{type.isSystem ? 'Да' : 'Нет'}</td>
                                    <td>
                                        <button type="button" onClick={() => startEdit(type)}>
                                            Изменить
                                        </button>
                                        {!type.isSystem && (
                                            <button type="button" onClick={() => handleDelete(type.id)}>
                                                Удалить
                                            </button>
                                        )}
                                    </td>
                                </>
                            )}
                        </tr>
                    ))}

                    {/* Строка добавления нового типа */}
                    <tr>
                        <td>
                            <input
                                value={newName}
                                onChange={(e) => setNewName(e.target.value)}
                                placeholder="Название нового типа"
                                disabled={creating}
                            />
                        </td>
                        <td>
                            <input
                                type="checkbox"
                                checked={newActive}
                                onChange={(e) => setNewActive(e.target.checked)}
                                disabled={creating}
                            />
                        </td>
                        <td>—</td>
                        <td>
                            <button
                                type="button"
                                onClick={handleCreate}
                                disabled={creating || !newName.trim()}
                            >
                                Добавить
                            </button>
                            {createError && <span style={{ color: 'red' }}> {createError}</span>}
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    );
}