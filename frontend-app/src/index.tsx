import React from 'react';
import ReactDOM from 'react-dom/client';
import './styles/index.css';
import App from './components/App';
import reportWebVitals from './reportWebVitals';
import {ConfigProvider} from "antd";
import ru_RU from 'antd/locale/ru_RU';
import BrochureStore from "./stores/BrochureStore";
import {Provider} from "mobx-react";

/**
 * Корень приложения.
 */
const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

/**
 * Набор классов хранилищ.
 */
const stores = {
    brochureStore: new BrochureStore()
};

/**
 * Отрисовка корня.
 */
root.render(
  <React.StrictMode>
      <Provider {...stores}>
          <ConfigProvider locale={ru_RU}>
              <App />
          </ConfigProvider>
      </Provider>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
