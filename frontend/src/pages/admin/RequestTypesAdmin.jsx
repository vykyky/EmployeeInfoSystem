export default function RequestTypesAdmin() {
  return (
    <div>
      <h2>Электронные запросы</h2>

      <button>Добавить</button>

      <hr />

      <ul>
        <li>
          Справка о доходах
          <button>Изменить</button>
          <button>Удалить</button>
        </li>

        <li>
          Справка о стаже
          <button>Изменить</button>
          <button>Удалить</button>
        </li>
      </ul>
    </div>
  )
}