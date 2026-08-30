import { useState, useEffect } from 'react';
import { getActiveRequestTypes } from '../../api/requestTypesApi';
import { createRequest } from '../../api/requestsApi';
import './Requests.css';

export default function Requests() {
  const [types, setTypes] = useState([]);
  const [requestTypeId, setRequestTypeId] = useState('');
  const [comment, setComment] = useState('');
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  useEffect(() => {
    getActiveRequestTypes()
      .then(data => {
        setTypes(data);
        if (data.length > 0) setRequestTypeId(String(data[0].id));
        setLoading(false);
      })
      .catch(err => { setError(err.message); setLoading(false); });
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    if (!requestTypeId) { setError('Выберите тип запроса'); return; }

    setSubmitting(true);
    setError(null);
    setSuccess(null);

    const selectedType = types.find(t => String(t.id) === String(requestTypeId));

    try {
      await createRequest({
        requestTypeId: parseInt(requestTypeId, 10),
        comment: comment.trim() || null,
      });
      setComment('');
      setSuccess(
        selectedType
          ? `Запрос «${selectedType.name}» принят`
          : 'Запрос принят'
      );
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) return <div className="page">Загрузка...</div>;

  return (
    <div className="page">
      <div className="req-card">
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="request-type">Тип запроса</label>
            <select
              id="request-type"
              value={requestTypeId}
              onChange={(e) => setRequestTypeId(e.target.value)}
              disabled={submitting || types.length === 0}
            >
              {types.length === 0 && <option value="">Нет доступных типов</option>}
              {types.map(t => (
                <option key={t.id} value={t.id}>{t.name}</option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label htmlFor="request-comment">Комментарий</label>
            <textarea
              id="request-comment"
              placeholder="Дополнительная информация..."
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              rows={4}
              disabled={submitting}
            />
          </div>

          <button
            type="submit"
            className="btn btn-primary"
            style={{ fontSize: 15 }}
            disabled={submitting || !requestTypeId}
          >
            {submitting ? 'Отправка...' : 'Запросить'}
          </button>
        </form>

        {error && <p className="req-error">{error}</p>}
        {success && <p className="req-success">{success}</p>}
      </div>
    </div>
  );
}