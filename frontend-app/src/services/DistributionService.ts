import BaseService from "./BaseService";
import {BACKEND_CONTROLLER_ROUTE} from "../constants/Routes";

/**
 * Класс сервис для обращению к контроллеру рассылок.
 */
class DistributionService extends BaseService {
    /**
     * Отправляет запрос на создание рассылки.
     * @param params Параметры рассылки.
     */
    public static async createDistribution(params: object) {
        const methodRoute = `/createDistribution`;
        return await BaseService.sendPostHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на изменение рассылки.
     * @param id Идентификатор рассылки.
     * @param params Параметры рассылки.
     */
    public static async updateDistribution(id: number, params: object) {
        const methodRoute = `/updateDistribution/id=${id}`;
        return await BaseService.sendPostHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на удаление рассылки.
     * @param id Идентификатор рассылки.
     */
    public static async deleteDistribution(id: number) {
        const methodRoute = `/deleteDistribution/id=${id}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Возвращает список возрастных групп.
     */
    public static async getAgeGroups() {
        const methodRoute = `/getAgeGroups`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }
}

export default DistributionService;