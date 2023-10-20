import BaseService from "./BaseService";
import {BACKEND_CONTROLLER_ROUTE} from "../constants/Routes";

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
     * Отправляет запрос на удаление товара из каталога.
     * @param goodId Идентификатор товара.
     * @param brochureId Идентификатор каталога.
     */
    public static async deleteBrochureGood(goodId: number, brochureId: number) {
        const methodRoute = `/deleteBrochureGood/goodId=${goodId}/brochureId=${brochureId}`;
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }
}

export default GoodsService;