import { useState } from "react";
import { useNavigate, NavLink, Outlet } from "react-router-dom";
import Header from "../components/Header";
import { clearAuth } from "../api/authApi";
import NotificationsBadge from "../components/NotificationsBadge";

export default function ManagerLayout() {
  const navigate = useNavigate();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  function handleLogout() {
    clearAuth();
    navigate("/");
  }

  function closeSidebar() {
    setSidebarOpen(false);
  }

  return (
    <div>
      <Header onBurgerClick={() => setSidebarOpen(prev => !prev)} />
      <div className="layout">

        {sidebarOpen && (
          <div className="sidebar-overlay" onClick={closeSidebar} />
        )}

        <aside className={`sidebar ${sidebarOpen ? "sidebar--open" : ""}`}>
          <ul className="sidebar-menu">
            <li>
              <NavLink to="/manager/notifications" onClick={closeSidebar}>
                Уведомления
                <NotificationsBadge />
              </NavLink>
            </li>
            <li>
              <NavLink to="/manager/news" onClick={closeSidebar}>Новости</NavLink>
            </li>
            <li>
              <NavLink to="/manager/tasks" onClick={closeSidebar}>Задачи</NavLink>
            </li>
            <li className="logout-item">
              <button onClick={handleLogout}>Выход</button>
            </li>
          </ul>
        </aside>

        <main className="content">
          <Outlet />
        </main>
      </div>
    </div>
  );
}