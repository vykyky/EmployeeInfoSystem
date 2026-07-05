import { NavLink } from "react-router-dom"
import "./Footer.css"

export default function Footer() {
  return (
    <footer className="app-footer">
      <NavLink to="/news">Новости</NavLink>
      Тут наверное надо что то написать
    </footer>
  )
}