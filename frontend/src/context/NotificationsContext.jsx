import { createContext, useContext, useState, useEffect, useCallback } from 'react';
import { getMyNotifications, markNotificationRead, deleteNotification } from '../api/notificationsApi';

const NotificationsContext = createContext(null);

export function NotificationsProvider({ children }) {
  const [items, setItems] = useState([]);
  const [loaded, setLoaded] = useState(false);

  const load = useCallback(() => {
    getMyNotifications()
      .then(data => {
        setItems(data);
        setLoaded(true);
      })
      .catch(() => setLoaded(true));
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const unreadCount = items.filter(n => !n.isRead).length;

  // Пометить одно уведомление прочитанным локально + на сервере
  const markRead = useCallback((id) => {
    setItems(prev =>
      prev.map(n => n.id === id ? { ...n, isRead: true } : n)
    );
    markNotificationRead(id).catch(() => {});
  }, []);

  // Пометить прочитанными сразу несколько уведомлений (по списку id).
  // Используется страницей уведомлений при уходе с неё — передаются id тех
  // карточек, что реально были видны пользователю на экране.
  const markManyRead = useCallback((ids) => {
    if (!ids || ids.length === 0) return;

    const idSet = new Set(ids);
    setItems(prev =>
      prev.map(n => (idSet.has(n.id) && !n.isRead) ? { ...n, isRead: true } : n)
    );

    ids.forEach(id => markNotificationRead(id).catch(() => {}));
  }, []);

  // Удалить уведомление локально + на сервере
  const removeNotification = useCallback((id) => {
    setItems(prev => prev.filter(n => n.id !== id));
    deleteNotification(id).catch(() => {});
  }, []);

  return (
    <NotificationsContext.Provider
      value={{ items, unreadCount, loaded, load, markRead, markManyRead, removeNotification }}
    >
      {children}
    </NotificationsContext.Provider>
  );
}

export function useNotifications() {
  const ctx = useContext(NotificationsContext);
  if (!ctx) throw new Error('useNotifications must be used inside NotificationsProvider');
  return ctx;
}