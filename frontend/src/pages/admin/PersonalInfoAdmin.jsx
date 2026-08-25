import { useState } from 'react';
import { syncProfileByTabn, syncAllProfiles } from '../../api/syncApi';
import './SyncAdmin.css';

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
    <div className="page">

      <div className="sync-card">
        <div className="form-group">
          <label>Номер лицевого счета для синхронизации личных данных</label>
          <input
            placeholder="Лицевой счет"
            value={tabn}
            onChange={(e) => setTabn(e.target.value)}
            disabled={loading}
          />
        </div>

        <div className="sync-actions">
          <button
            className="btn btn-primary"
            onClick={() => handleSync(() => syncProfileByTabn(tabn))}
            disabled={loading || !tabn.trim()}
          >
            Обновить сотрудника
          </button>

          <button
            className="btn btn-secondary"
            onClick={() => handleSync(syncAllProfiles)}
            disabled={loading}
          >
            Обновить всех сотрудников
          </button>
        </div>

        {loading && <p className="sync-status">Выполняется синхронизация...</p>}
        {status && (
          <p className={`sync-status ${status.ok ? 'sync-status-ok' : 'sync-status-error'}`}>
            {status.message}
          </p>
        )}
      </div>
    </div>
  );
}