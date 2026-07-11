package {{package}};

import {{package}}.models.{{ClassName}}; // Import da sua entidade
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface {{ClassName}}Repository extends JpaRepository<{{ClassName}}, Long> {
    
    
}