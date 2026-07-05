import { useNavigate } from "react-router-dom"

export default function Tasks() {
  const navigate = useNavigate()

  return (
    <div>
      <h2>Задачи</h2>

      <table border="1">
        <tbody>

          <tr onClick={() => navigate("/manager/tasks/1")}>
            <td>Справка о доходах</td>
            <td>Новая</td>
          </tr>

          <tr onClick={() => navigate("/manager/tasks/2")}>
            <td>Изменение телефона</td>
            <td>В работе</td>
          </tr>

        </tbody>
      </table>
    </div>
  )
}