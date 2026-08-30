import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'
import "./styles/app.css"
import "./styles/buttons.css"
import "./styles/forms.css"
import "./styles/layout.css"
import "./styles/tables.css"
import { NotificationsProvider } from './context/NotificationsContext.jsx'
import { subscribeToPush, sendSubscriptionToServer } from './api/pushApi.js'
import { HelmetProvider } from 'react-helmet-async'

// Регистрация SW
if ('serviceWorker' in navigator) {
  window.addEventListener('load', async () => {
    try {
      const reg = await navigator.serviceWorker.register('/sw.js');
      console.log('SW registered:', reg.scope);

      // Подписка на пуш (только если пользователь залогинен)
      const token = localStorage.getItem('token');
      if (token) {
        const sub = await subscribeToPush();
        if (sub) await sendSubscriptionToServer(sub).catch(() => {});
      }
    } catch (err) {
      console.warn('SW registration failed:', err);
    }
  });
}

createRoot(document.getElementById('root')).render(
  <StrictMode>
      <NotificationsProvider>
        <App />
      </NotificationsProvider>
  </StrictMode>,
)