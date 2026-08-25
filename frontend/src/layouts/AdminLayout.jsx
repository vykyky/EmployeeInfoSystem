import { useNavigate, NavLink, Outlet } from "react-router-dom"
import Header from "../components/Header"
import Footer from "../components/Footer"
import { clearAuth } from "../api/authApi";

export default function AdminLayout({ children }) {
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
              <NavLink to="/admin/workwear">
                Спец. одежда
              </NavLink>
            </li>

            <li>
              <NavLink to="/admin/personal-info">
                Личная информация
              </NavLink>
            </li>

            <li>
              <NavLink to="/admin/notifications">
                Уведомления
              </NavLink>
            </li>

            <li>
              <NavLink to="/admin/tasks">
                Задачи
              </NavLink>
            </li>

            

            <li>
              <NavLink to="/admin/request-types">
                Электронный запрос
              </NavLink>
            </li>
            <li>
             <NavLink to="/admin/users">
                Пользователи
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