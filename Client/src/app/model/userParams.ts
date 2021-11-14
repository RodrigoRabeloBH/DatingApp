import { User } from "./user";

export class USerParams {
    gender!: string;
    minAge = 18;
    maxAge = 99;
    pageNumber = 1;
    pageSize = 6;
    orderBy: string = 'lastActive';

    constructor(user: User) {
        this.gender = user.gender === 'female' ? 'male' : 'female';
    }
}