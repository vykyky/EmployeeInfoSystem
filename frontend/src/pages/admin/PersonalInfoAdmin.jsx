import { useState } from 'react';
import { syncProfileByTabn, syncAllProfiles } from '../../api/syncApi';

export default function PersonalInfoAdmin() {
  const [tabn, setTabn]       = useState('');
  const [status, setStatus]   = useState(null);   // { ok: bool, message: string }
  const [loading, setLoading] = useState(false);

  async function handleSync(syncFn) {
    setLoading(true);
    setStatus(null);
    try {
      await syncFn();
      setStatus({ ok: true, message: "Синхронизация выполнена успешно" });
    } catch (err) {
      setStatus({ ok: false, message: err.message });
    } finally {
      setLoading(false);
    }
  }

  return (
    <div>
      <h2>Личная информация</h2>

      <input
        placeholder="Лицевой счет"
        value={tabn}
        onChange={(e) => setTabn(e.target.value)}
        disabled={loading}
      />

      <br /><br />

      <button
        onClick={() => handleSync(() => syncProfileByTabn(tabn))}
        disabled={loading || !tabn.trim()}
      >
        Обновить сотрудника
      </button>

      <button
        onClick={() => handleSync(syncAllProfiles)}
        disabled={loading}
      >
        Обновить всех сотрудников
      </button>

      {loading && <p>Выполняется синхронизация...</p>}
      {status && (
        <p style={{ color: status.ok ? 'green' : 'red' }}>
          {status.message}
        </p>
      )}
    </div>
  );
}