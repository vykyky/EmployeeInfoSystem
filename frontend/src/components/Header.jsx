import { NavLink, useLocation } from "react-router-dom";
import logo from "../assets/logo.png";
import "./Header.css";

export default function Header() {
  const isAuthenticated = !!localStorage.getItem("token");
  const location = useLocation();

  return (
    <header className="app-header">
      <img src={logo} alt="logo" />

      {!isAuthenticated && (
        <NavLink
          to={location.pathname === "/news" ? "/" : "/news"}
          className="header-news-link"
        >
          {location.pathname === "/news"
            ? "Войти"
            : "Новости предприятия"}
        </NavLink>
      )}
    </header>
  );
}