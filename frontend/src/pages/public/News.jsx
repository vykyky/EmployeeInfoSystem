import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Header from "../../components/Header";
import Footer from "../../components/Footer"
import { getAllNews } from "../../api/newsApi";
import { API_URL } from "../../config/api";

import "./News.css";

export default function News() {
  const navigate = useNavigate();

  const [news, setNews] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const isAuthenticated = !!localStorage.getItem("token");
  const newsBasePath = isAuthenticated ? "/user/news" : "/news";

  // пагинация
  const [page, setPage] = useState(1);
  const pageSize = 8;

  useEffect(() => {
    async function loadNews() {
      try {
        const data = await getAllNews();
        setNews(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadNews();
  }, []);

  const start = (page - 1) * pageSize;
  const currentNews = news.slice(start, start + pageSize);
  const totalPages = Math.ceil(news.length / pageSize);

  if (loading) return <p>Загрузка...</p>;
  if (error) return <p>{error}</p>;

  return (
    <div className="news-page">
    <div className="news-main">
     
      {!isAuthenticated && (
          <div className="news-header-row">
            <h1 className="news-title">Новости предприятия</h1>
            
          </div>
        )}

      {news.length === 0 && <p>Новостей пока нет</p>}

      <div className="news-grid">
        {currentNews.map((item) => (
          <div className="news-card" key={item.id}>

            <div
              className="news-image"
              onClick={() => navigate(`${newsBasePath}/${item.id}`)}
            >
              {item.imagePath && (
                <img src={`${API_URL}/${item.imagePath}`} />
              )}
            </div>
            <small className="news-date">
              {new Date(item.createdAt).toLocaleDateString("ru-RU")}
            </small>
            <h2
              className="news-card-title"
              onClick={() => navigate(`${newsBasePath}/${item.id}`)}
            >
              {item.title}
            </h2>

            <p className="news-text">
              {item.body.slice(0, 120)}...
            </p>

            
          </div>
        ))}
      </div>

      {/* пагинация */}
      <div className="pagination">
        <button disabled={page === 1} onClick={() => setPage(p => p - 1)}>
          Назад
        </button>

        <span>
          {page} / {totalPages}
        </span>

        <button
          disabled={page === totalPages}
          onClick={() => setPage(p => p + 1)}
        >
          Вперёд
        </button>
      </div>
    </div>
    </div>
  );
}