import Header from "../components/Header"
import Footer from "../components/Footer"
import { useNavigate, NavLink, Outlet } from "react-router-dom"
import { clearAuth } from "../api/authApi";


export default function UserLayout({ children }) {
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
           
            <li><NavLink to="/user/notifications">Уведомления</NavLink></li>
            <li><NavLink to="/user/requests">Электронный запрос</NavLink></li>
            <li><NavLink to="/user/workwear">Спец. одежда</NavLink></li>
            <li><NavLink to="/user/news">Новости</NavLink></li>
            <li><NavLink to="/user/profile">Профиль</NavLink></li>

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