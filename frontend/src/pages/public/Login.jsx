import { useNavigate } from "react-router-dom"
import { useState } from "react"
import Header from "../../components/Header"
import Footer from "../../components/Footer"
import { login, saveAuth } from "../../api/authApi"
import "./Login.css"
import { FaEye, FaEyeSlash } from "react-icons/fa"

export default function Login() {
  const navigate = useNavigate()

  const [tabn, setTabn] = useState("")
  const [password, setPassword] = useState("")
  const [error, setError] = useState("")
  const [loading, setLoading] = useState(false)
  const [showPassword, setShowPassword] = useState(false)
  
  const handleAuth = async () => {
    setError("")

    if (!tabn.trim() || !password) {
      setError("Введите табельный номер и пароль")
      return
    }

    setLoading(true)
    try {
      const data = await login(tabn.trim(), password)
      saveAuth(data.token, data.role, data.fio)

      if (data.role === "admin") navigate("/admin")
      else if (data.role === "manager") navigate("/manager")
      else navigate("/user")

    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  const handleKeyDown = (e) => {
    if (e.key === "Enter") handleAuth()
  }

  return (
    <div className="login-page">

      <main className="login-main">
        <div className="login-container">

          <h1 className="page-title">Вход в личный кабинет</h1>

          <div className="form-group">
            <label>Табельный номер</label>
            <input
              type="text"
              value={tabn}
              onChange={(e) => setTabn(e.target.value)}
              onKeyDown={handleKeyDown}
              disabled={loading}
            />
          </div>

         <div className="form-group password-group">
          <label>Пароль</label>

          <div className="input-wrapper">
            <input
              type={showPassword ? "text" : "password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              onKeyDown={handleKeyDown}
              disabled={loading}
            />

            <button
              type="button"
              className="eye-btn"
              onClick={() => setShowPassword(prev => !prev)}
            >
              {showPassword ? <FaEyeSlash /> : <FaEye />}
            </button>
          </div>
        </div>

          {error && <p className="login-error">{error}</p>}

          <div className="login-links">
            <a>Восстановить пароль</a>
          </div>

          <button
            className="btn btn-primary"
            onClick={handleAuth}
            disabled={loading}
          >
            {loading ? "Вход..." : "Войти"}
          </button>


        </div>
      </main>

    </div>
  )
}