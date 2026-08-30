import { useEffect, useRef, useCallback } from 'react';
import { useNotifications } from '../context/NotificationsContext';
import './NotificationsInbox.css';

export default function NotificationsInbox() {
  const { items, loaded, markManyRead, removeNotification } = useNotifications();

  // Сюда копим id карточек, которые реально были видны на экране
  const seenIds = useRef(new Set());
  const observerRef = useRef(null);

  // Создаём IntersectionObserver один раз при монтировании страницы
  useEffect(() => {
    observerRef.current = new IntersectionObserver(
      (entries) => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            const id = Number(entry.target.dataset.id);
            seenIds.current.add(id);
          }
        });
      },
      { threshold: 0.5 } // карточка должна быть видна минимум наполовину
    );

    return () => {
      observerRef.current?.disconnect();
    };
  }, []);

  // ref-callback, которым подписываем каждую карточку на observer
  const itemRef = useCallback((node) => {
    if (node && observerRef.current) {
      observerRef.current.observe(node);
    }
  }, []);

  // При уходе со страницы уведомлений — помечаем прочитанными
  // всё, что реально успело промелькнуть на экране
  useEffect(() => {
    return () => {
      markManyRead(Array.from(seenIds.current));
    };
  }, [markManyRead]);

  const formatDate = (d) => (d ? new Date(d).toLocaleString('ru-RU') : '');

  if (!loaded) return <div className="page">Загрузка...</div>;

  // Непрочитанные — сверху, дальше по дате (новые выше)
  const sortedItems = [...items].sort((a, b) => {
    if (a.isRead !== b.isRead) return a.isRead ? 1 : -1;
    return new Date(b.createdAt) - new Date(a.createdAt);
  });

  function handleDelete(e, id) {
    e.stopPropagation();
    seenIds.current.delete(id);
    removeNotification(id);
  }

  return (
    <div className="page">
      {sortedItems.length === 0 ? (
        <p className="ni-empty">Уведомлений нет</p>
      ) : (
        <ul className="ni-list">
          {sortedItems.map(item => (
            <li
              key={item.id}
              data-id={item.id}
              ref={itemRef}
              className={`ni-item${item.isRead ? ' ni-item--read' : ''}`}
            >
              <button
                type="button"
                className="ni-item-delete"
                onClick={(e) => handleDelete(e, item.id)}
                aria-label="Удалить уведомление"
                title="Удалить"
              >
                ✕
              </button>

              <div className="ni-item-title">{item.title}</div>
              <div className="ni-item-body">{item.body}</div>
              <div className="ni-item-date">{formatDate(item.createdAt)}</div>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}