import BaseService from "./BaseService";
import {BACKEND_CONTROLLER_ROUTE} from "../constants/Routes";
import {CreateBrochureHandlerProps} from "../types/BrochureTypes";

/**
 * Сервис для отправки запросов к backend.
 */
class BrochureService extends BaseService {
    /**
     * Возвращает с backend все каталоги.
     */
    public static async getAllBrochures() {
        const methodRoute = "/getBrochures";
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Возвращает данные каталога по идентификатору каталога.
     * @param id Идентифкиатор каталога.
     */
    public static async getBrochureById(id: number) {
        const methodRoute = `/getBrochure/id=${id}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Возвращает товары каталога по идентификатору каталога.
     * @param brochureId Идентифкиатор каталога.
     */
    public static async getBrochureDistributions(brochureId: number) {
        const methodRoute = `/getBrochureDistributions/id=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Отправляет запрос на создание каталога.
     * @param params Параметры каталога.
     */
    public static async createBrochure(params: CreateBrochureHandlerProps) {
        const methodRoute = `/createBrochure`;
        return await BaseService.sendPostHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на изменение каталога.
     * @param id Идентификатор каталога.
     * @param params Параметры каталога.
     */
    public static async updateBrochure(id: number, params: object) {
        const methodRoute = `/updateBrochure/id=${id}`;
        return await BaseService.sendPostHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute, params);
    }

    /**
     * Отправляет запрос на удаление каталога.
     * @param id Идентификатор каталога.
     */
    public static async deleteBrochure(id: number) {
        const methodRoute = `/deleteBrochure/id=${id}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }
}

export default BrochureService;