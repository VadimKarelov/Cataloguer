import BaseService from "./BaseService";
import {BACKEND_CONTROLLER_ROUTE} from "../constants/Routes";

/**
 * Сервис для отправки запросов раздела аудит к backend.
 */
class AuditService extends BaseService {
    /**
     * Возвращает с backend все логги.
     */
    public static async getLogs() {
        const methodRoute = "/getLogs";
        return await BaseService.sendGetHttpRequest(BACKEND_CONTROLLER_ROUTE, methodRoute);
    }
}

export default AuditService;