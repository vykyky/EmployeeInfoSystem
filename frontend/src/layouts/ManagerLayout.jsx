import Header from "../components/Header"
import Footer from "../components/Footer"
import { useNavigate, NavLink, Outlet } from "react-router-dom"
import { clearAuth } from "../api/authApi";

export default function ManagerLayout({ children }) {
  const navigate = useNavigate()

  function handleLogout() {
    clearAuth()
    navigate("/")
  }

  return (
    <div>
      <Header />

      <div className="layout">

        <aside className="sidebar">
         

          <ul className="sidebar-menu">
            <li>
              <NavLink to="/manager/news">
                Новости
              </NavLink>
            </li>

            <li>
              <NavLink to="/manager/tasks">
                Задачи
              </NavLink>
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
  )
}