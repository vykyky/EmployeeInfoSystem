export default function TaskDetails() {
  return (
    <div>
      <h2>Задача</h2>

      <div>
        <label>Наименование задачи</label>
        <br />
        <input
          value="Справка о доходах"
          readOnly
        />
      </div>

      <br />

      <div>
        <label>Комментарии пользователя</label>
        <br />
        <textarea
          value="Нужна справка за 2025 год"
          readOnly
        />
      </div>

      <br />

      <div>
        <label>Описание решения</label>
        <br />
        <textarea />
      </div>

      <br />

      <button>В работу</button>
      <button>Отправить</button>
      <button>Завершить</button>
    </div>
  )
}