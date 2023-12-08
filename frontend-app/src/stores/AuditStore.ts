import { observable, action, makeAutoObservable } from "mobx";
import {AuditDbProps} from "../types/AuditTypes";
import AuditService from "../services/AuditService";
import dayjs from "dayjs";
import {cerr} from "../Utils";

/**
 * Класс хранилище для раздела аудита.
 */
class AuditStore {
    /**
     * Множество логов.
     */
    @observable public logs: AuditDbProps[];

    /**
     * Загружаются ли логи.
     */
    @observable public isLoadingLogs: boolean;

    /**
     * Конструктор.
     */
    constructor() {
        this.logs = [];
        this.isLoadingLogs = false;

        makeAutoObservable(this);
        this.getLogs = this.getLogs.bind(this);
        this.updateCurrentLogs = this.updateCurrentLogs.bind(this);
    }

    /**
     * Возвращает логи.
     * @private
     */
    private async getLogs() {
        return await AuditService.getLogs();
    }

    /**
     * Обновляет список логов.
     */
    @action public async updateCurrentLogs() {
        this.isLoadingLogs = true;

        await this.getLogs().then(
            (response) => {
                const data = response.data;
                if (!(data instanceof Array)) {
                    this.isLoadingLogs = false;
                    return;
                }
                this.logs = data.map((row: AuditDbProps) => ({...row, dateTime: dayjs(row.dateTime).format('DD.MM.YYYY HH:mm:ss')}));
            },
            (error) => cerr(error),
        ).finally(() => {
            setTimeout(() => this.isLoadingLogs = false, 300);
        });
    }
}

export default AuditStore;