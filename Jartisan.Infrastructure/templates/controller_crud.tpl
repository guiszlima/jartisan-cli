package {{package}}.controllers;

import {{package}}.models.{{ClassName}};
import {{package}}.services.{{ClassName}}Service;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/{{endpointName}}")
public class {{ClassName}}Controller {

    private final {{ClassName}}Service service;

    
    public {{ClassName}}Controller({{ClassName}}Service service) {
        this.service = service;
    }

    // Listar todos (Index)
    @GetMapping
    public ResponseEntity<List<{{ClassName}}>> index() {
        return ResponseEntity.ok(service.findAll());
    }

    // Criar novo (Store)
    @PostMapping
    public ResponseEntity<{{ClassName}}> store(@RequestBody {{ClassName}} entity) {
        {{ClassName}} created = service.save(entity);
        return new ResponseEntity<>(created, HttpStatus.CREATED);
    }

    // Visualizar um (Show)
    @GetMapping("/{id}")
    public ResponseEntity<{{ClassName}}> show(@PathVariable Long id) {
        return service.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    // Atualizar (Update)
    @PutMapping("/{id}")
    public ResponseEntity<{{ClassName}}> update(@PathVariable Long id, @RequestBody {{ClassName}} entity) {
        return ResponseEntity.ok(service.update(id, entity));
    }

    // Deletar (Destroy)
    @DeleteMapping("/{id}")
    public ResponseEntity<Void> destroy(@PathVariable Long id) {
        service.delete(id);
        return ResponseEntity.noContent().build();
    }
}