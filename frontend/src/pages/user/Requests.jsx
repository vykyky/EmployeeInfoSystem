import { useState, useEffect } from 'react';
import { getActiveRequestTypes } from '../../api/requestTypesApi';
import { createRequest } from '../../api/requestsApi';

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
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    if (!requestTypeId) {
      setError('Выберите тип запроса');
      return;
    }

    setSubmitting(true);
    setError(null);
    setSuccess(null);

    const selectedType = types.find(t => String(t.id) === String(requestTypeId));

    try {
      await createRequest({
        requestTypeId: parseInt(requestTypeId, 10),
        comment: comment.trim() || null
      });
      setComment('');
      setSuccess(
        selectedType
          ? `Электронный запрос «${selectedType.name}» принят`
          : 'Электронный запрос принят'
      );
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  }

  if (loading) return <div>Загрузка...</div>;

  return (
    <div>
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
          <label htmlFor="request-comment">Комментарии</label>
          <textarea
            id="request-comment"
            placeholder="Комментарии..."
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            rows={4}
            disabled={submitting}
          />
        </div>

        <button
          type="submit"
          className="btn btn-primary"
          disabled={submitting || !requestTypeId}
        >
          Запросить
        </button>
      </form>

      {error && <p style={{ color: 'red', marginTop: 12 }}>{error}</p>}
      {success && <p style={{ color: 'green', marginTop: 12 }}>{success}</p>}
    </div>
  );
}
