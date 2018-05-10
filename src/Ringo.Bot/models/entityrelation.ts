export interface EntityRelationship {
    FromVertex: {
        Id: string,
        Name: string,
        Properties: {}
    },
    ToVertex: {
        Id: string,
        Name: string,
        Properties: {}
    },
    Relationship: string,
    RelationshipDate: Date
}