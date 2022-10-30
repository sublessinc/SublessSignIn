import { ICreatorMessage } from "./ICreatorMessage";

export class CreatorMessage implements ICreatorMessage {
    constructor(
        public message: string,
        public creatorId: string
    ) { }
}