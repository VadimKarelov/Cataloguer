import axios from "axios";

/**
 * Базовый класс сервис.
 * Имеет методы для отправки http запросов к backend.
 */
class BaseService {
    /**
     * Отправляет GET запрос.
     * @param baseRoute Маршрут контроллера.
     * @param subRoute Маршрут метода.
     * @protected
     */
    protected static async sendGetHttpRequest(baseRoute: Readonly<string>, subRoute: Readonly<string>) {
        const route: Readonly<string> = `${baseRoute}${subRoute}`;
        return await axios.get(route);
    }

    /**
     * Отправляет POST запрос.
     * @param baseRoute Маршрут контроллера.
     * @param subRoute Маршрут метода.
     * @param params Параметры запроса.
     * @protected
     */
    protected static async sendPostHttpRequest(baseRoute: Readonly<string>, subRoute: Readonly<string>, params: object) {
        const route: Readonly<string> = `${baseRoute}${subRoute}`;
        // const formData = new FormData();

        // возможно косячная штука
        // Object.keys(params).forEach(key => {
        //     const parsableObject: customProp = params;
        //     const value = parsableObject[key];
        //     if (value) {
        //         formData.append(key, value);
        //     }
        // });
        // return await axios.post(route, formData);
        return await axios.post(route, JSON.stringify(params));
    }
}

export default BaseService;