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
    }

    /**
     * Обрабатывает редактирование рассылки текущего каталога.
     * @param params Параметры запроса.
     */
    @action  public async handleEditBrochureDistribution(params: EditDistributionDbProps) {
        const id = params.id;
        if (id === -1) return;

        this.isLoadingDistributions = true;

        console.log(params)

        await DistributionService.updateDistribution(id, params);
        this.updateBrochureDistributions(params.brochureId);
    }

    /**
     * Обрабатывает создание рассылки текущего каталога.
     * @param params Параметры запроса.
     */
    @action public async handleCreateBrochureDistribution(params: CreateDistributionDbProps) {
        this.isLoadingDistributions = true;
        await DistributionService.createDistribution(params);
        this.updateBrochureDistributions(params.brochureId);
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
        const {data} = await this.getBrochureDistributions(brochureId);
        if (!(data instanceof Array)) {
            this.isLoadingDistributions = false;
            return;
        }

        this.distributions = data;
        setTimeout(() => this.isLoadingDistributions = false, 300);
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
        ]).then((responses) => {
            const ageGroupsAxiosResponse = responses[0];
            const gendersAxiosResponse = responses[1];
            const townsAxiosResponse = responses[2];

            if (responses.some(response => response.status >= 400)) return;

            this.genders = gendersAxiosResponse.data;
            this.ageGroups = ageGroupsAxiosResponse.data;
            this.towns = townsAxiosResponse.data;
        });
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