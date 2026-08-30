import { useNotifications } from '../context/NotificationsContext';
import './NotificationsBadge.css';

export default function NotificationsBadge() {
  const { unreadCount } = useNotifications();

  if (unreadCount === 0) return null;

  return (
    <span className="notif-badge">
      {unreadCount > 99 ? '99+' : unreadCount}
    </span>
  );
}