import { useNavigate } from "react-router-dom"

export default function TasksAdmin() {
  const navigate = useNavigate()

  return (
    <div>
      <h2>Все задачи</h2>

      <table border="1">
        <thead>
          <tr>
            <th>Задача</th>
            <th>Статус</th>
            <th>Менеджер</th>
          </tr>
        </thead>

        <tbody>

          <tr
            onClick={() =>
              navigate("/admin/tasks/1")
            }
          >
            <td>Справка о доходах</td>
            <td>Новая</td>
            <td>Иванов</td>
          </tr>

          <tr
            onClick={() =>
              navigate("/admin/tasks/2")
            }
          >
            <td>Изменение телефона</td>
            <td>В работе</td>
            <td>Петров</td>
          </tr>

        </tbody>
      </table>
    </div>
  )
}