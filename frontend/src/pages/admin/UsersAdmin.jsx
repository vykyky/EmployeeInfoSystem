import { useEffect, useState } from "react";
import { getAllUsers, createUser, deleteUser, changeUserRole } from "../../api/usersApi";
import "./UsersAdmin.css";
import { FaEye, FaEyeSlash } from "react-icons/fa"

const ROLES = ["employee", "manager", "admin"];

function validate(form) {
    const errors = {};

    if (!form.tabn.trim())
        errors.tabn = "Введите табельный номер";

    if (!form.password)
        errors.password = "Введите пароль";
    else if (form.password.length < 6)
        errors.password = "Пароль не менее 6 символов";

    if (!form.role)
        errors.role = "Выберите роль";

    return errors;
}

export default function UsersAdmin() {
    const [users, setUsers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");


    // форма регистрации
    const [showForm, setShowForm] = useState(false);
    const [form, setForm] = useState({ tabn: "", password: "", role: "employee" });
    const [formErrors, setFormErrors] = useState({});
    const [formLoading, setFormLoading] = useState(false);
    const [formError, setFormError] = useState("");
    const [showPassword, setShowPassword] = useState(false)

    // смена роли
    const [roleEditing, setRoleEditing] = useState(null);
    const [newRole, setNewRole] = useState("");
    const [roleLoading, setRoleLoading] = useState(false);

    // удаление
    const [deleteConfirm, setDeleteConfirm] = useState(null);
    const [deleteLoading, setDeleteLoading] = useState(false);

    useEffect(() => {
        loadUsers();
    }, []);

    async function loadUsers() {
        setLoading(true);
        setError("");
        try {
            const data = await getAllUsers();
            setUsers(data);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    }

    function handleFormChange(e) {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
        if (formErrors[name])
            setFormErrors(prev => ({ ...prev, [name]: "" }));
    }

    async function handleCreate(e) {
        e.preventDefault();
        const errors = validate(form);
        if (Object.keys(errors).length > 0) {
            setFormErrors(errors);
            return;
        }

        setFormLoading(true);
        setFormError("");
        try {
            await createUser(form);
            setForm({ tabn: "", password: "", role: "employee" });
            setShowForm(false);
            await loadUsers();
        } catch (err) {
            setFormError(err.message);
        } finally {
            setFormLoading(false);
        }
    }

    function handleCancelForm() {
        setShowForm(false);
        setForm({ tabn: "", password: "", role: "employee" });
        setFormErrors({});
        setFormError("");
    }

    function startRoleEdit(user) {
        setRoleEditing(user.id);
        setNewRole(user.role);
    }

    async function handleRoleSave(id) {
        setRoleLoading(true);
        try {
            await changeUserRole(id, newRole);
            setUsers(prev => prev.map(u => u.id === id ? { ...u, role: newRole } : u));
            setRoleEditing(null);
        } catch (err) {
            alert(err.message);
        } finally {
            setRoleLoading(false);
        }
    }

    async function handleDelete(id) {
        setDeleteLoading(true);
        try {
            await deleteUser(id);
            setUsers(prev => prev.filter(u => u.id !== id));
            setDeleteConfirm(null);
        } catch (err) {
            alert(err.message);
        } finally {
            setDeleteLoading(false);
        }
    }

    if (loading) return <p>Загрузка...</p>;
    if (error) return <p className="users-error">{error}</p>;

    return (
        <div className="page">
            <div className="page-header">
              
                <button
                    className="btn btn-primary"
                    onClick={() => setShowForm(true)}
                >
                    Зарегистрировать
                </button>
            </div>

            {showForm && (
                <div className="users-form-wrap">
                    <h2 className="users-form-title">Новый пользователь</h2>
                    <form onSubmit={handleCreate} noValidate>
                        <div className="form-group">
                            <label>Табельный номер</label>
                            <input
                                name="tabn"
                                value={form.tabn}
                                onChange={handleFormChange}
                                placeholder="000001"
                                autoComplete="off"
                            />
                            {formErrors.tabn && <span className="users-field-error">{formErrors.tabn}</span>}
                        </div>

                       
                        <div className="form-group password-group">
                            <label>Пароль</label>
                            <div className="input-wrapper">
                                <input
                                    name="password"
                                    type={showPassword ? "text" : "password"}
                                    value={form.password}
                                    onChange={handleFormChange}
                                    placeholder="Минимум 6 символов"
                                />
                            
                                <button
                                    type="button"
                                    className="eye-btn"
                                    onClick={() => setShowPassword(prev => !prev)}
                                    >
                                    {showPassword ? <FaEyeSlash /> : <FaEye />}
                                </button>
                            </div>
                            {formErrors.password && <span className="users-field-error">{formErrors.password}</span>}   
                        </div>      

                        <div className="form-group">
                            <label>Роль</label>
                            <select
                                name="role"
                                value={form.role}
                                onChange={handleFormChange}
                            >
                                {ROLES.map(r => (
                                    <option key={r} value={r}>{r}</option>
                                ))}
                            </select>
                            {formErrors.role && <span className="users-field-error">{formErrors.role}</span>}
                        </div>

                        {formError && <p className="users-field-error">{formError}</p>}

                        <div className="users-form-actions">
                            
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={handleCancelForm}
                                disabled={formLoading}
                            >
                                Отмена
                            </button>
                            <button
                                type="submit"
                                className="btn btn-primary"
                                disabled={formLoading}
                            >
                                {formLoading ? "Сохранение..." : "Зарегистрировать"}
                            </button>
                        </div>
                    </form>
                </div>
            )}

            {users.length === 0 ? (
                <p>Пользователей пока нет</p>
            ) : (
                <div className="users-table-wrap">
                    <table className="table">
                        <thead>
                            <tr>
                                <th>Табельный №</th>
                                <th>ФИО</th>
                                <th>Роль</th>
                                <th>Дата регистрации</th>
                                <th>Последний вход</th>
                                <th>Действия</th>
                            </tr>
                        </thead>
                        <tbody>
                            {users.map(user => (
                                <tr key={user.id}>
                                    <td>{user.tabn}</td>
                                    <td>{user.fio ?? "—"}</td>

                                    <td>
                                        {roleEditing === user.id ? (
                                            <div className="users-role-edit">
                                                <select
                                                    value={newRole}
                                                    onChange={e => setNewRole(e.target.value)}
                                                >
                                                    {ROLES.map(r => (
                                                        <option key={r} value={r}>{r}</option>
                                                    ))}
                                                </select>
                                                <button
                                                    className="btn btn-primary users-btn-sm"
                                                    onClick={() => handleRoleSave(user.id)}
                                                    disabled={roleLoading}
                                                >
                                                    ✓
                                                </button>
                                                <button
                                                    className="btn btn-secondary users-btn-sm"
                                                    onClick={() => setRoleEditing(null)}
                                                    disabled={roleLoading}
                                                >
                                                    ✕
                                                </button>
                                            </div>
                                        ) : (
                                            <span
                                                className="users-role-label"
                                                onClick={() => startRoleEdit(user)}
                                                title="Нажмите чтобы изменить роль"
                                            >
                                                {user.role}
                                            </span>
                                        )}
                                    </td>

                                    <td>
                                        {new Date(user.createdAt).toLocaleDateString("ru-RU")}
                                    </td>

                                    <td>
                                        {user.lastLoginAt
                                            ? new Date(user.lastLoginAt).toLocaleDateString("ru-RU")
                                            : "Не входил"}
                                    </td>

                                    <td>
                                        {deleteConfirm === user.id ? (
                                            <div className="users-delete-confirm">
                                                <span>Удалить?</span>
                                                <button
                                                    className="btn btn-delete users-btn-sm"
                                                    onClick={() => handleDelete(user.id)}
                                                    disabled={deleteLoading}
                                                >
                                                    Да
                                                </button>
                                                <button
                                                    className="btn btn-secondary users-btn-sm"
                                                    onClick={() => setDeleteConfirm(null)}
                                                    disabled={deleteLoading}
                                                >
                                                    Нет
                                                </button>
                                            </div>
                                        ) : (
                                            <button
                                                className="btn btn-delete users-btn-sm"
                                                onClick={() => setDeleteConfirm(user.id)}
                                            >
                                                Удалить
                                            </button>
                                        )}
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}