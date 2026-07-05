import Header from "../components/Header"
import Footer from "../components/Footer"
import { Outlet } from "react-router-dom"

export default function PublicLayout({ children }) {
  return (
    <div className="public-layout">
      <Header />
      <main className="public-content"><Outlet /></main>
      <Footer />
    </div>
  )
}