import { observable, action, makeAutoObservable } from "mobx";
import DistributionService from "../services/DistributionService";

interface AgeGroupDbProps {
    id: number,
    description: string,
    minimalAge: number,
    maximalAge: number,
}

interface TownDbProps {
    id: number,
    name: string,
    population: number,
}

interface GenderDbProps {
    id: number,
    name: string,
}

export class DistributionStore {
    @observable private genders: GenderDbProps[];
    @observable private towns: TownDbProps[];
    @observable private ageGroups: AgeGroupDbProps[];

    constructor() {
        this.genders = [];
        this.towns = [];
        this.ageGroups = [];

        makeAutoObservable(this);

        this.getAgeGroups = this.getAgeGroups.bind(this);
        this.getGenders = this.getGenders.bind(this);
        this.getTowns = this.getTowns.bind(this);
        this.updateDistributionLists = this.updateDistributionLists.bind(this);
    }

    /**
     * Обновляет список возврастных группы.
     */
    @action public updateDistributionLists(): void {
        Promise.all([
            this.getAgeGroups(),
            this.getGenders(),
            this.getTowns(),
        ]).then((responses) => {
            console.log(responses)
            const ageGroupsAxiosResponse = responses[0];
            const gendersAxiosResponse = responses[1];
            const townsAxiosResponse = responses[2];

            if (responses.some(response => response.status >= 400)) return;

            this.genders = gendersAxiosResponse.data;
            this.ageGroups = ageGroupsAxiosResponse.data;
            this.towns = townsAxiosResponse.data;

            console.log(this.genders)
            console.log(this.ageGroups)
            console.log(this.towns)
        });
    }

    private async getGenders() {
        return await DistributionService.getGenders();
    }

    private async getTowns() {
        return await DistributionService.getTowns();
    }

    /**
     * Возвращает возрастные группы.
     */
    private async getAgeGroups() {
        return await DistributionService.getAgeGroups();
    }
}