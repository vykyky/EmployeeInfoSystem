import React, { useState, useEffect } from 'react';
import { getWorkwear, sendWorkwearUpdateRequest } from '../../api/workwearApi';
import './Workwear.css';

export default function Workwear() {
  const [workwear, setWorkwear] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [isEditing, setIsEditing] = useState(false);
  const [clothesSize, setClothesSize] = useState('');
  const [shoesSize, setShoesSize] = useState('');

  const [submitting, setSubmitting] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [submitError, setSubmitError] = useState(null);

  useEffect(() => {
    getWorkwear()
      .then(data => {
        setWorkwear(data);
        setClothesSize(data.clothesSize ?? '');
        setShoesSize(data.shoesSize ?? '');
        setLoading(false);
      })
      .catch(err => { setError(err.message); setLoading(false); });
  }, []);

  const isChanged =
    clothesSize !== (workwear?.clothesSize ?? '') ||
    shoesSize !== (workwear?.shoesSize ?? '');

  const handleSendRequest = async () => {
    if (!isChanged) return;
    setSubmitting(true);
    setSubmitError(null);
    setSuccessMessage('');
    try {
      await sendWorkwearUpdateRequest(clothesSize, shoesSize);
      setSuccessMessage('Запрос на изменение размеров создан.');
      setIsEditing(false);
    } catch (err) {
      setSubmitError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  const handleCancel = () => {
    setClothesSize(workwear?.clothesSize ?? '');
    setShoesSize(workwear?.shoesSize ?? '');
    setSubmitError(null);
    setIsEditing(false);
  };

  const formatDate = (d) => (d ? new Date(d).toLocaleDateString('ru-RU') : '—');

  const formatWearPeriod = (months) => {
    if (months === null || months === undefined) return '—';
    if (months === 65535) return 'До износа';
    return `${months} мес.`;
  };

  if (loading) return <div className="page">Загрузка...</div>;
  if (error) return <div className="page ww-error">Ошибка: {error}</div>;

  return (
    <div className="page">

    

      {/* Таблица выданной одежды */}
      {workwear.items && workwear.items.length > 0 ? (
        <div className="ww-table-wrap">
          <table className="table">
            <thead>
              <tr>
                <th>Наименование</th>
                <th>Группа</th>
                <th>Дата выдачи</th>
                <th>Дата окончания</th>
                <th>Количество</th>
                <th>Срок носки</th>
              </tr>
            </thead>
            <tbody>
              {workwear.items.map((item, index) => (
                <tr key={index}>
                  <td>{item.itemName || '—'}</td>
                  <td>{item.groupName || '—'}</td>
                  <td>{formatDate(item.giveDate)}</td>
                  <td>{formatDate(item.endDate)}</td>
                  <td>{item.quantity ?? '—'}</td>
                  <td>{formatWearPeriod(item.wearPeriod)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : (
        <p className="ww-empty">Спец. одежда не выдана</p>
      )}

        {/* Размеры */}
      <div className="ww-card">
        {successMessage && <p className="ww-success">{successMessage}</p>}
        {submitError && <p className="ww-submit-error">Ошибка: {submitError}</p>}

        <div className="ww-sizes">
        <div className="form-group">
          <label>Размер одежды</label>
          <input
            type="number"
            value={clothesSize}
            onChange={(e) => setClothesSize(e.target.value === '' ? '' : Number(e.target.value))}
            disabled={!isEditing || submitting}
            placeholder="—"
            style={{ maxWidth: 120 }}
          />
        </div>

        <div className="form-group">
          <label>Размер обуви</label>
          <input
            type="number"
            value={shoesSize}
            onChange={(e) => setShoesSize(e.target.value === '' ? '' : Number(e.target.value))}
            disabled={!isEditing || submitting}
            placeholder="—"
            style={{ maxWidth: 120 }}
          />
        </div>
        </div>
        <div className="ww-actions">
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
              Изменить размер
            </button>
          )}
        </div>

        <p className="ww-hint">
          Изменение размеров производится через подачу запроса. Изменения будут видны после обработки.
        </p>
      </div>

    </div>
  );
}