import React, { useState, useEffect } from 'react';
import { getWorkwear, sendWorkwearUpdateRequest } from '../../api/workwearApi';

export default function Workwear() {
  const [workwear, setWorkwear] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Стейты редактирования размеров
  const [isEditing, setIsEditing] = useState(false);
  const [clothesSize, setClothesSize] = useState('');
  const [shoesSize, setShoesSize] = useState('');

  // Стейты отправки формы
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
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
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
      
      setSuccessMessage('Запрос на изменение размеров спецодежды успешно создан!');
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

  const formatDate = (dateString) => {
    if (!dateString) return '—';
    return new Date(dateString).toLocaleDateString('ru-RU');
  };

  const formatWearPeriod = (months) => {
    if (months === null || months === undefined) return '—';
    if (months === 65535) return 'До износа';
    return `${months} мес.`;
  };

  if (loading) return <div>Загрузка данных по спец. одежде...</div>;
  if (error) return <div>Ошибка: {error}</div>;

  return (
    <div>
      <h2>Спец. одежда</h2>

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
          <label>Размер одежды </label>
          <input
            type="number"
            value={clothesSize}
            onChange={(e) => setClothesSize(e.target.value === '' ? '' : Number(e.target.value))}
            disabled={!isEditing || submitting}
            placeholder="—"
          />
        </div>

        <div>
          <label>Размер обуви </label>
          <input
            type="number"
            value={shoesSize}
            onChange={(e) => setShoesSize(e.target.value === '' ? '' : Number(e.target.value))}
            disabled={!isEditing || submitting}
            placeholder="—"
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
              Изменить размер
            </button>
          )}
        </div>
      </div>

      <div style={{ fontSize: '0.85em', color: '#666', marginTop: '15px', marginBottom: '20px' }}>
        *Изменение размеров производится через подачу запроса. Изменения будут видны после обработки запроса.
      </div>

      <h4>Выданная спец. одежда</h4>
      {/* Остальной код таблицы оставляем прежним */}
      {workwear.items && workwear.items.length > 0 ? (
        <table border="1">
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
      ) : (
        <p>Спец. одежда не выдана</p>
      )}
    </div>
  );
}