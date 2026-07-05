import React, { useState, useEffect } from 'react';
// Импортируем обе функции
import { getProfile, sendProfileUpdateRequest } from '../../api/profileApi'; 

export default function Profile() {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Стейты для редактирования
  const [isEditing, setIsEditing] = useState(false);
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');

  // Стейты для процесса отправки и фидбека
  const [submitting, setSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [submitError, setSubmitError] = useState(null);

  useEffect(() => {
    getProfile()
      .then(data => {
        setProfile(data);
        setPhone(data.phone || '');
        setEmail(data.email || '');
        setLoading(false);
      })
      .catch(err => {
        setError(err.message); 
        setLoading(false);
      });
  }, []);

  const isChanged = 
    phone !== (profile?.phone || '') || 
    email !== (profile?.email || '');

  const handleSendRequest = async () => {
    if (!isChanged) return;

    setSubmitting(true);
    setSubmitError(null);
    setSuccessMessage('');

    try {
      // Вызываем реальный API-запрос
      await sendProfileUpdateRequest(phone, email);
      
      // Если всё ок, выводим красивый статус
      setSuccessMessage('Запрос на изменение контактов успешно зарегистрирован и отправлен менеджеру!');
      setIsEditing(false);
    } catch (err) {
      // Если бэкенд вернул ошибку, покажем её
      setSubmitError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  const handleCancel = () => {
    setPhone(profile.phone || '');
    setEmail(profile.email || '');
    setSubmitError(null);
    setIsEditing(false);
  };

  const formatDate = (dateString) => {
    if (!dateString) return '—';
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  if (loading) return <div>Загрузка профиля...</div>;
  if (error) return <div>Ошибка: {error}</div>;

  return (
    <div>
      <h2>Личная информация</h2>
      <div>
        <p><strong>ФИО:</strong> {profile.fio || 'Не указано'}</p>
        <p><strong>Дата рождения:</strong> {formatDate(profile.bornDate)}</p>
        <p><strong>Дата приема:</strong> {formatDate(profile.hireDate)}</p>
      </div>

      <h2>Настройка пользователя</h2>

      {/* Вывод системных сообщений под заголовком */}
      {successMessage && (
        <div style={{ color: 'green', backgroundColor: '#e6ffe6', padding: '10px', borderRadius: '4px', marginBottom: '10px' }}>
          ✓ {successMessage}
        </div>
      )}
      {submitError && (
        <div style={{ color: 'red', backgroundColor: '#ffe6e6', padding: '10px', borderRadius: '4px', marginBottom: '10px' }}>
          ⚠ Ошибка при отправке: {submitError}
        </div>
      )}
      
      <div>
        <div>
          <label>Телефон </label>
          <input 
            type="text" 
            value={phone} 
            onChange={(e) => setPhone(e.target.value)}
            disabled={!isEditing || submitting} 
            placeholder="+375 (XX) XXX-XX-XX"
          />
        </div>

        <div>
          <label>Email </label>
          <input 
            type="email" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)}
            disabled={!isEditing || submitting} 
            placeholder="example@mail.com"
          />
        </div>

        <div style={{ marginTop: '10px' }}>
          {isEditing ? (
            <>
              <button 
                type="button" 
                onClick={handleSendRequest}
                disabled={!isChanged || submitting}
              >
                {submitting ? 'Отправка...' : 'Отправить запрос на изменение'}
              </button>
              <button type="button" onClick={handleCancel} disabled={submitting} style={{ marginLeft: '8px' }}>
                Отмена
              </button>
            </>
          ) : (
            <button type="button" onClick={() => { setIsEditing(true); setSuccessMessage(''); }}>
              Изменить данные
            </button>
          )}
        </div>
      </div>

      <div style={{ fontSize: '0.85em', color: '#666', marginTop: '15px' }}>
        *Изменение номера телефона и e-mail производится через подачу запроса. Изменения будут видны после обработки запроса.
      </div>
    </div>
  );
}