import { NavLink } from "react-router-dom"
import "./Footer.css"

export default function Footer() {
  return (
    <footer className="app-footer">
      <NavLink to="/news">О системе</NavLink>
      Разработка ООО "Топ Софт"
    </footer>
  )
}