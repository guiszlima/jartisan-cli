using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jartisan.Infrastructure.Implementations.Maven
{
    public class MavenOrgRegistry
    {
        public static IReadOnlySet<string> OfficialOrgPrefixes { get; } = new HashSet<string>
        {
           // --- Frameworks, Linguagem e Core ---
    "org.springframework",
    "io.quarkus",
    "io.micronaut",
    "jakarta.enterprise",
    "org.jetbrains.kotlin", // Líder absoluto em ecossistema alternativo à JVM

    // --- Utilitários e Produtividade ---
    "org.projectlombok",
    "com.fasterxml.jackson",
    "com.google.guava",
    "com.google.code.gson",
    "org.apache.commons",
    "org.mapstruct",

    // --- Banco de Dados e ORM ---
    "org.hibernate",
    "org.postgresql",
    "com.mysql",
    "org.mongodb",
    "com.h2database", // Adicionado: Banco em memória muito usado para testes

    // --- Testes ---
    "org.junit",
    "org.mockito",
    "org.assertj",
    "org.testcontainers", // Adicionado: O queridinho atual para testes de integração reais

    // --- Log, Resiliência e Telemetria ---
    "org.slf4j",
    "ch.qos.logback",
    "org.apache.logging.log4j",
    "io.github.resilience4j",
    "io.micrometer", 
    "io.opentelemetry",

    // --- Nuvem, Mensageria e Integrações ---
    "org.springdoc",
    "org.apache.kafka",
    "com.rabbitmq",
    "io.netty",
    "io.jsonwebtoken", // Core de segurança (JWT)
    "org.keycloak",

    // --- Cloud Providers SDKs ---
    "software.amazon.awssdk",
    "com.amazonaws",
    "com.azure",
    "com.google.cloud",

    // --- Big Data ---
    "org.apache.spark",
    "org.apache.hadoop"
        };
    }
}