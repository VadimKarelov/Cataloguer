import BaseService from "./BaseService";
import {BACKEND_CONTROLLER_ROUTE} from "../constants/Routes";
import {GoodDBProps} from "../types/BrochureTypes";

/**
 * Класс сервис для обращению к контроллеру товаров каталога.
 */
class GoodsService extends BaseService {
    /**
     * Возвращает товары каталога по идентификатору каталога.
     * @param brochureId Идентифкиатор каталога.
     */
    public static async getBrochureGoods(brochureId: number) {
        const methodRoute = `/getBrochureGoods/id=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Возвращает все невыбранные товары каталога по идентификатору каталога.
     * @param brochureId Идентифкиатор каталога.
     */
    public static async getUnselectedBrochureGoods(brochureId: number) {
        const methodRoute = `/getUnselectedBrochureGoods/id=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Отправляет запрос на добавление товаров в каталог.
     * @param brochureId Идентифкиатор каталога.
     * @param goods Добавляемые товары.
     */
    public static async addGoodsToBrochure(brochureId: number, goods: GoodDBProps[]) {
        const methodRoute = `/addGoodsToBrochure/id=${brochureId}`;
        return await BaseService.sendPostHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute, goods);
    }

    /**
     * Отправляет запрос на удаление товара из каталога.
     * @param goodId Идентификатор товара.
     * @param brochureId Идентификатор каталога.
     */
    public static async deleteBrochureGood(goodId: number, brochureId: number) {
        const methodRoute = `/deleteBrochureGood/id=${goodId}&brochureId=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }

    /**
     * Отправляет запрос на получение всех возможных товаров.
     */
    public static async getGoods() {
        const methodRoute = `/getGoods`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }
}

export default GoodsService;