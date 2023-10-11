import BaseService from "./BaseService";

/**
 * Сервис для отправки запросов к backend.
 */
class BrochureService extends BaseService {
    /**
     * Основной путь обращения к контроллеру.
     */
    private BACKEND_BASE_ROUTE: Readonly<string> = "/api/v1/cataloguer";

    /**
     * Возвращает с backend все каталоги.
     */
    public async getAllBrochures() {
        const methodRoute = "/getBrochures";
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }

    /**
     * Возвращает данные каталога по идентификатору каталога.
     * @param id Идентифкиатор каталога.
     */
    public async getBrochureById(id: number) {
        const methodRoute = `/getBrochure/id=${id}`;
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }

    /**
     * Возвращает товары каталога по идентификатору каталога.
     * @param brochureId Идентифкиатор каталога.
     */
    public async getBrochureDistributions(brochureId: number) {
        const methodRoute = `/getDistributions/id=${brochureId}`;
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }

    /**
     * Отправляет запрос на создание каталога.
     * @param params Параметры каталога.
     */
    public async createBrochure(params: object) {
        const methodRoute = `/createBrochure`;
        return await this.sendPostHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на изменение каталога.
     * @param id Идентификатор каталога.
     * @param params Параметры каталога.
     */
    public async updateBrochure(id: number, params: object) {
        const methodRoute = `/updateBrochure/id=${id}`;
        return await this.sendPostHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на удаление каталога.
     * @param id Идентификатор каталога.
     */
    public async deleteBrochure(id: number) {
        const methodRoute = `/deleteBrochure/id=${id}`;
        return await this.sendGetHttpRequest(this.BACKEND_BASE_ROUTE, methodRoute);
    }
}

export default BrochureService;