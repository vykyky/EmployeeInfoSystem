import { useState, useEffect } from 'react';
import { getMyNotifications, markNotificationRead } from '../api/notificationsApi';

export default function NotificationsInbox() {
  const [items, setItems] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    load();
  }, []);

  function load() {
    setLoading(true);
    getMyNotifications()
      .then(data => {
        setItems(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message);
        setLoading(false);
      });
  }

  async function handleOpen(item) {
    if (!item.isRead) {
      try {
        await markNotificationRead(item.id);
        setItems(prev => prev.map(n =>
          n.id === item.id ? { ...n, isRead: true } : n
        ));
      } catch {
        // список всё равно показываем
      }
    }
  }

  const unread = items.filter(n => !n.isRead).length;
  const formatDate = (d) => (d ? new Date(d).toLocaleString('ru-RU') : '');

  if (loading) return <div>Загрузка...</div>;
  if (error) return <div>Ошибка: {error}</div>;

  return (
    <div>
      <h2>Уведомления</h2>
      <p>Непрочитанные: {unread}</p>

      {items.length === 0 ? (
        <p>Нет уведомлений</p>
      ) : (
        <ul style={{ listStyle: 'none', padding: 0, maxWidth: 640, margin: '0 auto', textAlign: 'left' }}>
          {items.map(item => (
            <li
              key={item.id}
              onClick={() => handleOpen(item)}
              style={{
                padding: '10px 12px',
                marginBottom: 8,
                background: item.isRead ? 'transparent' : 'rgba(84, 107, 142, 0.12)',
                borderRadius: 8,
                cursor: 'pointer',
                border: '1px solid #ddd'
              }}
            >
              <div style={{ fontWeight: item.isRead ? 400 : 600 }}>{item.title}</div>
              <div>{item.body}</div>
              <div style={{ fontSize: 13, color: '#666', marginTop: 4 }}>{formatDate(item.createdAt)}</div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}
