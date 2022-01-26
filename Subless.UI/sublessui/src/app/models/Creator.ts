import { ICreator } from "./ICreator";

export class Creator implements ICreator {
    constructor(
        public username: string,
        public payPalId: string,
        public id: string
    ) { }
}