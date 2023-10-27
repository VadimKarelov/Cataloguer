import { observable, action, makeAutoObservable } from "mobx";
import DistributionService from "../services/DistributionService";

/**
 * Свойства возрастной группы из БД.
 * @param id Идентификатор.
 * @param description Описание.
 * @param minimalAge Минимальный возраст для группы.
 * @param maximalAge Максимальный возраст для группы.
 */
interface AgeGroupDbProps {
    id: number,
    description: string,
    minimalAge: number,
    maximalAge: number,
}

/**
 * Свойства города из БД.
 * @param id Идентификатор.
 * @param name Наименование.
 * @param population Число жителей.
 */
interface TownDbProps {
    id: number,
    name: string,
    population: number,
}

/**
 * Свойства пола из БД.
 * @param id Идентификатор.
 * @param name Наименование.
 */
interface GenderDbProps {
    id: number,
    name: string,
}

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
     * Конструктор.
     */
    constructor() {
        this.genders = [];
        this.towns = [];
        this.ageGroups = [];

        makeAutoObservable(this);

        this.getDbAgeGroups = this.getDbAgeGroups.bind(this);
        this.getDbGenders = this.getDbGenders.bind(this);
        this.getDbTowns = this.getDbTowns.bind(this);
        this.getGenders = this.getGenders.bind(this);
        this.getTowns = this.getTowns.bind(this);
        this.getAgeGroups = this.getAgeGroups.bind(this);
        this.updateDistributionLists = this.updateDistributionLists.bind(this);
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
        await Promise.all([
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