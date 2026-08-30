import { useEffect, useState } from "react";
import { getAllNews, createNews, updateNews, deleteNews } from "../../api/newsApi";
import { API_URL } from "../../config/api";
import './NewsManager.css';
import './CreateNews.css'; // переиспользуем стили формы (превью картинки, news-form-*)

export default function NewsManager() {
  const [news, setNews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [deleteConfirm, setDeleteConfirm] = useState(null);

  // модалка создания/редактирования
  const [modalMode, setModalMode] = useState(null); // null | 'create' | 'edit'
  const [editingId, setEditingId] = useState(null);

  const [title, setTitle] = useState("");
  const [body, setBody] = useState("");
  const [image, setImage] = useState(null);
  const [preview, setPreview] = useState(null);
  const [submitting, setSubmitting] = useState(false);
  const [formError, setFormError] = useState(null);

  useEffect(() => {
    loadNews();
  }, []);

  function loadNews() {
    setLoading(true);
    getAllNews()
      .then(data => setNews(data))
      .finally(() => setLoading(false));
  }

  // Закрытие модалки по Escape — как в RequestTypesAdmin/UsersAdmin
  useEffect(() => {
    function handleKeyDown(e) {
      if (e.key === 'Escape' && modalMode) {
        handleCloseModal();
      }
    }
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [modalMode]);

  function resetForm() {
    setTitle("");
    setBody("");
    setImage(null);
    setPreview(null);
    setFormError(null);
    setEditingId(null);
  }

  function openCreateModal() {
    resetForm();
    setModalMode('create');
  }

  function openEditModal(item) {
    resetForm();
    setEditingId(item.id);
    setTitle(item.title);
    setBody(item.body);
    if (item.imagePath) setPreview(`${API_URL}/${item.imagePath}`);
    setModalMode('edit');
  }

  function handleCloseModal() {
    if (submitting) return;
    setModalMode(null);
    resetForm();
  }

  function handleImageChange(e) {
    const file = e.target.files[0];
    if (!file) return;
    setImage(file);
    setPreview(URL.createObjectURL(file));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!title.trim()) { setFormError('Введите заголовок'); return; }

    setSubmitting(true);
    setFormError(null);
    try {
      const formData = new FormData();
      formData.append("title", title);
      formData.append("body", body);
      if (image) formData.append("image", image);

      if (modalMode === 'create') {
        formData.append("authorId", 1);
        await createNews(formData);
      } else {
        formData.append("id", editingId);
        await updateNews(editingId, formData);
      }

      setModalMode(null);
      resetForm();
      loadNews();
    } catch (err) {
      setFormError(err.message);
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(id) {
    try {
      await deleteNews(id);
      setNews(prev => prev.filter(n => n.id !== id));
      setDeleteConfirm(null);
    } catch (error) {
      alert(error.message);
    }
  }

  if (loading) return <div className="page">Загрузка...</div>;

  return (
    <div className="page">
      <div className="page-header">
        <button className="btn btn-primary" onClick={openCreateModal}>
          Создать новость
        </button>
      </div>

      {news.length === 0 ? (
        <p className="nm-empty">Новостей пока нет</p>
      ) : (
        <div className="nm-list">
          {news.map(item => (
            <div key={item.id} className="nm-item">
              <span className="nm-item-title">{item.title}</span>
              <div className="nm-item-actions">
                {deleteConfirm === item.id ? (
                  <>
                    <span className="nm-confirm-text">Удалить?</span>
                    <button
                      className="btn btn-delete"
                      style={{ fontSize: 13, padding: '4px 12px' }}
                      onClick={() => handleDelete(item.id)}
                    >
                      Да
                    </button>
                    <button
                      className="btn btn-secondary"
                      style={{ fontSize: 13, padding: '4px 12px' }}
                      onClick={() => setDeleteConfirm(null)}
                    >
                      Нет
                    </button>
                  </>
                ) : (
                  <>
                    <button
                      className="btn btn-secondary"
                      style={{ fontSize: 13, padding: '4px 12px' }}
                      onClick={() => openEditModal(item)}
                    >
                      Редактировать
                    </button>
                    <button
                      className="btn btn-delete"
                      style={{ fontSize: 13, padding: '4px 12px' }}
                      onClick={() => setDeleteConfirm(item.id)}
                    >
                      Удалить
                    </button>
                  </>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {modalMode && (
        <div className="modal-overlay" onClick={handleCloseModal}>
          <div className="modal-card news-modal-card" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h2 className="modal-title">
                {modalMode === 'create' ? 'Новая новость' : 'Редактирование новости'}
              </h2>
              <button
                type="button"
                className="modal-close-btn"
                onClick={handleCloseModal}
                disabled={submitting}
              >
                ✕
              </button>
            </div>

            <form onSubmit={handleSubmit} noValidate>
              <div className="news-form-image-wrap">
                {preview ? (
                  <img src={preview} alt="Превью" className="news-form-preview" />
                ) : (
                  <div className="news-form-image-placeholder">Изображение не выбрано</div>
                )}
                <label className="btn btn-secondary news-form-file-btn" style={{ fontSize: 14 }}>
                  {modalMode === 'create' ? 'Выбрать изображение' : 'Изменить изображение'}
                  <input type="file" accept="image/*" onChange={handleImageChange} hidden />
                </label>
              </div>

              <div className="form-group">
                <label>Заголовок</label>
                <input
                  value={title}
                  onChange={e => setTitle(e.target.value)}
                  placeholder="Заголовок новости"
                  disabled={submitting}
                  autoFocus
                />
              </div>

              <div className="form-group">
                <label>Текст</label>
                <textarea
                  value={body}
                  onChange={e => setBody(e.target.value)}
                  placeholder="Текст новости..."
                  rows={6}
                  disabled={submitting}
                />
              </div>

              {formError && <p className="news-form-error">{formError}</p>}

              <div className="news-form-actions">
                <button
                  type="submit"
                  className="btn btn-primary"
                  style={{ fontSize: 15 }}
                  disabled={submitting || !title.trim()}
                >
                  {submitting ? 'Сохранение...' : 'Сохранить'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}