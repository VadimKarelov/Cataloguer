import {action, makeAutoObservable, observable} from "mobx";
import DistributionService from "../services/DistributionService";
import {
    AgeGroupDbProps,
    CreateDistributionDbProps,
    DistributionDbProps,
    EditDistributionDbProps,
    GenderDbProps,
    TownDbProps
} from "../types/DistributionTypes";

/**
 * Класс хранилище для рассылок.
 */
export class DistributionStore {
    /**
     * Множество полов.
     * @private
     */
    @observable private genders: GenderDbProps[];

    /**
     * Множество городов.
     * @private
     */
    @observable private towns: TownDbProps[];

    /**
     * Множество возрастных групп.
     * @private
     */
    @observable private ageGroups: AgeGroupDbProps[];

    /**
     * Список рассылок.
     */
    @observable public distributions: DistributionDbProps[];

    /**
     * Загружаются ли рассылки.
     */
    @observable public isLoadingDistributions: boolean;

    /**
     * Конструктор.
     */
    constructor() {
        this.genders = [];
        this.towns = [];
        this.ageGroups = [];
        this.distributions = [];
        this.isLoadingDistributions = false;

        makeAutoObservable(this);

        this.getDbAgeGroups = this.getDbAgeGroups.bind(this);
        this.getDbGenders = this.getDbGenders.bind(this);
        this.getDbTowns = this.getDbTowns.bind(this);
        this.getGenders = this.getGenders.bind(this);
        this.getTowns = this.getTowns.bind(this);
        this.getAgeGroups = this.getAgeGroups.bind(this);
        this.updateDistributionLists = this.updateDistributionLists.bind(this);

        this.getBrochureDistributions = this.getBrochureDistributions.bind(this);
        this.updateBrochureDistributions = this.updateBrochureDistributions.bind(this);
        this.handleCreateBrochureDistribution = this.handleCreateBrochureDistribution.bind(this);
        this.handleEditBrochureDistribution = this.handleEditBrochureDistribution.bind(this);
        this.handleDeleteBrochureDistribution = this.handleDeleteBrochureDistribution.bind(this);
    }

    /**
     * Обарабывает удаление рассылки каталога.
     * @param distributionId Идентификатор рассылки.
     * @param brochureId Идентификатор каталога.
     */
    @action public async handleDeleteBrochureDistribution(distributionId: number, brochureId: number): Promise<string> {
        if (brochureId === -1 || distributionId === -1) {
            return Promise.reject("Ошибка при удалении каталога");
        }

        return await DistributionService.deleteDistribution(distributionId).then(
            async(response) => {
                const data = response.data;
                await this.updateBrochureDistributions(brochureId);
                if (!isNaN(parseInt(data)) && parseInt(data) === -1) {
                    return Promise.reject("Ошибка при удалении каталога");
                }
                return Promise.resolve("Рассылка каталога удалёна успешно");
            },
            (error) => {
                console.log(error);
                return Promise.reject("Ошибка при удалении каталога");
            },
        );
    }

    /**
     * Обрабатывает редактирование рассылки текущего каталога.
     * @param params Параметры запроса.
     */
    @action  public async handleEditBrochureDistribution(params: EditDistributionDbProps): Promise<string> {
        const id = params.id;
        if (id === -1) {
            return Promise.reject("Не удалось изменить рассылку");
        }

        this.isLoadingDistributions = true;

        return await DistributionService.updateDistribution(id, params).then(
            async(response) => {
                const data = response.data;
                await this.updateBrochureDistributions(params.brochureId);

                if (data === -1) {
                    return Promise.reject("Не удалось изменить рассылку");
                }
                return Promise.resolve("Изменения сохранены");
            },
            (error) => {
                console.log(error);
                this.isLoadingDistributions = false;
                return Promise.reject("Не удалось изменить рассылку");
            },
        );
    }

    /**
     * Обрабатывает создание рассылки текущего каталога.
     * @param params Параметры запроса.
     */
    @action public async handleCreateBrochureDistribution(params: CreateDistributionDbProps): Promise<string> {
        this.isLoadingDistributions = true;
        return await DistributionService.createDistribution(params).then(
            async(response) => {
                const data = response.data;
                await this.updateBrochureDistributions(params.brochureId);

                if (data === -1) {
                    return Promise.reject("Не удалось создать рассылку");
                }
                return Promise.resolve("Рассылка успешно создана");
            },
            (error) => {
                console.log(error);
                this.isLoadingDistributions = false;
                return Promise.reject("Не удалось создать рассылку");
            }
        );
    }

    /**
     * Возвращает рассылки каталога по ИД каталога.
     * @param brochureId Идентификатор каталога.
     * @private
     */
    private async getBrochureDistributions(brochureId: number) {
        return DistributionService.getBrochureDistributions(brochureId);
    }

    /**
     * Обновляет список рассылок текущего каталога.
     * @param brochureId Идентификатор каталога.
     */
    @action public async updateBrochureDistributions(brochureId: number) {
        this.isLoadingDistributions = true;
        await this.getBrochureDistributions(brochureId).then(
            (response) => {
                const data = response.data;
                if (!(data instanceof Array)) {
                    this.isLoadingDistributions = false;
                    return;
                }

                this.distributions = data;
            },
            (error) => console.log(error),
        ).finally(() => setTimeout(() => this.isLoadingDistributions = false, 300));
    }

    /**
     * Возвращает все возможные значения для поля пол.
     */
    @action public getGenders = () => this.genders;

    /**
     * Возвращет все возможные значения для поля населённый пункт.
     */
    @action public getTowns = () => this.towns;

    /**
     * Вовзвращает все возможные значения для поля возрастная группа.
     */
    @action public getAgeGroups = () => this.ageGroups;

    /**
     * Обновляет список возврастных группы.
     */
    @action public async updateDistributionLists() {
        return await Promise.all([
            this.getDbAgeGroups(),
            this.getDbGenders(),
            this.getDbTowns(),
        ]).then(
            (responses) => {
                const ageGroupsAxiosResponse = responses[0];
                const gendersAxiosResponse = responses[1];
                const townsAxiosResponse = responses[2];

                if (responses.some(response => response.status >= 400)) return;

                this.genders = gendersAxiosResponse.data;
                this.ageGroups = ageGroupsAxiosResponse.data;
                this.towns = townsAxiosResponse.data;
            },
            (error) => console.log(error)
        );
    }

    /**
     * Возвращает из БД набор элементов пол.
     * @private
     */
    private async getDbGenders() {
        return await DistributionService.getGenders();
    }

    /**
     * Возвращает из БД города.
     * @private
     */
    private async getDbTowns() {
        return await DistributionService.getTowns();
    }

    /**
     * Возвращает возрастные группы.
     */
    private async getDbAgeGroups() {
        return await DistributionService.getAgeGroups();
    }
}