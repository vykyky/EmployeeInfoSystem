// src/api/pushApi.js
import { API_URL } from "../config/api"; // Добавляем импорт API_URL

function urlBase64ToUint8Array(base64String) {
  const padding = '='.repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding).replace(/-/g, '+').replace(/_/g, '/');
  const raw = atob(base64);
  return Uint8Array.from([...raw].map(c => c.charCodeAt(0)));
}

async function getVapidPublicKey() {
  const response = await fetch(`${API_URL}/api/push/vapid-public-key`);
  if (!response.ok) throw new Error('Не удалось получить VAPID ключ');
  return await response.text();
}

export async function subscribeToPush() {
  if (!('serviceWorker' in navigator) || !('PushManager' in window)) return null;

  const permission = await Notification.requestPermission();
  if (permission !== 'granted') return null;

  const reg = await navigator.serviceWorker.ready;
  let subscription = await reg.pushManager.getSubscription();

  if (!subscription) {
    const vapidPublicKey = await getVapidPublicKey();
    subscription = await reg.pushManager.subscribe({
      userVisibleOnly: true,
      applicationServerKey: urlBase64ToUint8Array(vapidPublicKey),
    });
  }

  return subscription;
}

export async function sendSubscriptionToServer(subscription) {
  if (!subscription) return;

  const token = localStorage.getItem('token');
  const jsonSub = subscription.toJSON();

  const payload = {
    endpoint: jsonSub.endpoint,
    keys: {
      p256dh: jsonSub.keys.p256dh,
      auth: jsonSub.keys.auth
    }
  };

  const response = await fetch(`${API_URL}/api/push/subscribe`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    console.error('Ошибка сохранения подписки на сервере:', await response.text());
  }
}