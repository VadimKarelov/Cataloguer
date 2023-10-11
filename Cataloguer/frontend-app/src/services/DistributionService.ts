import BaseService from "./BaseService";

/**
 * Класс сервис для обращению к контроллеру рассылок.
 */
class DistributionService extends BaseService {
    /**
     * Основной путь обращения к контроллеру.
     */
    private BACKEND_BASE_ROUTE: Readonly<string> = "/api/v1/distribution";

    /**
     * Отправляет запрос на создание рассылки.
     * @param params Параметры рассылки.
     */
    public async createDistribution(params: object) {
        const methodRoute = `/createDistribution`;
        return await this.sendPostHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на изменение рассылки.
     * @param id Идентификатор рассылки.
     * @param params Параметры рассылки.
     */
    public async updateDistribution(id: number, params: object) {
        const methodRoute = `/updateDistribution/id=${id}`;
        return await this.sendPostHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на удаление рассылки.
     * @param id Идентификатор рассылки.
     */
    public async deleteDistribution(id: number) {
        const methodRoute = `/deleteDistribution/id=${id}`;
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }
}

export default DistributionService;