import React, { useState, useEffect } from 'react';
import { getProfile, sendProfileUpdateRequest } from '../../api/profileApi';
import './Profile.css';

export default function Profile() {
  const [profile, setProfile] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [isEditing, setIsEditing] = useState(false);
  const [phone, setPhone] = useState('');
  const [email, setEmail] = useState('');

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
      .catch(err => { setError(err.message); setLoading(false); });
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
      await sendProfileUpdateRequest(phone, email);
      setSuccessMessage('Запрос на изменение контактов отправлен менеджеру.');
      setIsEditing(false);
    } catch (err) {
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

  const formatDate = (d) => (d ? new Date(d).toLocaleDateString('ru-RU') : '—');

  if (loading) return <div className="page">Загрузка...</div>;
  if (error) return <div className="page profile-error">Ошибка: {error}</div>;

  return (
    <div className="page">

      {/* Личная информация */}
      <div className="profile-card">
        <div className="profile-grid">
          <div className="profile-label">ФИО</div>
          <div className="profile-value">{profile.fio || '—'}</div>

          <div className="profile-label">Дата рождения</div>
          <div className="profile-value">{formatDate(profile.bornDate)}</div>

          <div className="profile-label">Дата приёма</div>
          <div className="profile-value">{formatDate(profile.hireDate)}</div>
        </div>
      </div>

      {/* Контакты */}
      <div className="profile-card" style={{ marginTop: 16 }}>
        {successMessage && (
          <p className="profile-success">{successMessage}</p>
        )}
        {submitError && (
          <p className="profile-submit-error">Ошибка: {submitError}</p>
        )}

        <div className="form-group">
          <label>Телефон</label>
          <input
            type="text"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
            disabled={!isEditing || submitting}
            placeholder="+375 (XX) XXX-XX-XX"
          />
        </div>

        <div className="form-group">
          <label>Email</label>
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            disabled={!isEditing || submitting}
            placeholder="example@mail.com"
          />
        </div>

        <div className="profile-actions">
          {isEditing ? (
            <>
              <button
                className="btn btn-primary"
                style={{ fontSize: 15 }}
                onClick={handleSendRequest}
                disabled={!isChanged || submitting}
              >
                {submitting ? 'Отправка...' : 'Отправить запрос'}
              </button>
              <button
                className="btn btn-secondary"
                style={{ fontSize: 15 }}
                onClick={handleCancel}
                disabled={submitting}
              >
                Отмена
              </button>
            </>
          ) : (
            <button
              className="btn btn-primary"
              style={{ fontSize: 15 }}
              onClick={() => { setIsEditing(true); setSuccessMessage(''); }}
            >
              Изменить данные
            </button>
          )}
        </div>

        <p className="profile-hint">
          Изменение телефона и e-mail производится через подачу запроса. Изменения будут видны после обработки.
        </p>
      </div>

    </div>
  );
}