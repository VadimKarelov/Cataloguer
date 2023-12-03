import React from 'react';
import ReactDOM from 'react-dom/client';
import './styles/index.css';
import App from './components/App';
import {ConfigProvider} from "antd";
import ru_RU from 'antd/locale/ru_RU';
import BrochureStore from "./stores/BrochureStore";
import {Provider} from "mobx-react";
import {DistributionStore} from "./stores/DistributionStore";
import GoodsStore from "./stores/GoodsStore";
import AuditStore from "./stores/AuditStore";

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
    brochureStore: new BrochureStore(),
    distributionStore: new DistributionStore(),
    goodsStore: new GoodsStore(),
    auditStore: new AuditStore(),
};

/**
 * Отрисовка корня.
 */
root.render(
  <React.StrictMode>
      <Provider {...stores}>
          <ConfigProvider
              theme={{
                  token: {
                      colorPrimary: '#d3a325',
                  },
              }}
              locale={ru_RU}
          >
              <App />
          </ConfigProvider>
      </Provider>
  </React.StrictMode>
);